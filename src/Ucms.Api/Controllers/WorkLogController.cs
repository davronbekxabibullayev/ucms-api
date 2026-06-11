namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/projects/{projectId:guid}/worklogs")]
[Tags("WorkLog")]
[Authorize]
public class WorkLogController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record CreateWorkLogRequest(
        Guid BrigadeId,
        Guid EstimateItemId,
        DateTimeOffset Date,
        decimal Volume,
        decimal? BrigadeUnitPrice,    // null → EstimateItem.BrigadeUnitPrice ishlatiladi
        string? Note);

    public record UpdateWorkLogRequest(
        DateTimeOffset Date,
        decimal Volume,
        decimal BrigadeUnitPrice,
        string? Note);

    public record ConfirmWorkLogRequest(Guid[] WorkLogIds);
    public record RejectWorkLogRequest(Guid[] WorkLogIds, string? Reason);

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task<Guid?> GetProjectOrgAsync(Guid projectId, CancellationToken ct) =>
        await db.Projects
            .Where(p => p.Id == projectId && !p.IsDeleted)
            .Select(p => (Guid?)p.OrganizationId)
            .FirstOrDefaultAsync(ct);

    private bool CanAccess(Guid orgId) =>
        ctx.IsOwner || ctx.OrganizationId == orgId;

    // ── GET /api/projects/{projectId}/worklogs/{id} ───────────────────────────

    /// <summary>
    /// Bitta ish jurnali yozuvi
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid projectId, Guid id, CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var workLog = await db.WorkLogs
            .Where(w => w.Id == id && w.ProjectId == projectId)
            .Select(w => new
            {
                w.Id, w.Date, w.Volume, w.BrigadeUnitPrice, w.TotalAmount,
                w.Status, w.Note, w.BrigadePaymentId,
                w.CreatedAt, w.UpdatedAt,
                Brigade = new { w.Brigade!.Id, w.Brigade.Name },
                EstimateItem = new
                {
                    w.EstimateItem!.Id,
                    w.EstimateItem.Name,
                    w.EstimateItem.Unit,
                },
            })
            .FirstOrDefaultAsync(ct);

        return workLog is null ? NotFound() : Ok(workLog);
    }

    // ── GET /api/projects/{projectId}/worklogs ─────────────────────────────────

    /// <summary>
    /// Loyiha ish jurnalidagi yozuvlar (filtrlash mumkin)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        Guid projectId,
        [FromQuery] Guid? brigadeId,
        [FromQuery] WorkLogStatus? status,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int page = 1,
        [FromQuery] int size = 50,
        CancellationToken ct = default)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var query = db.WorkLogs.Where(w => w.ProjectId == projectId);

        if (brigadeId.HasValue) query = query.Where(w => w.BrigadeId == brigadeId.Value);
        if (status.HasValue)    query = query.Where(w => w.Status == status.Value);
        if (from.HasValue)      query = query.Where(w => w.Date >= from.Value);
        if (to.HasValue)        query = query.Where(w => w.Date <= to.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(w => w.Date)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(w => new
            {
                w.Id, w.Date, w.Volume, w.BrigadeUnitPrice, w.TotalAmount,
                w.Status, w.Note, w.BrigadePaymentId,
                Brigade      = w.Brigade!.Name,
                EstimateItem = new { w.EstimateItem!.Name, w.EstimateItem.Unit },
            })
            .ToListAsync(ct);

        return Ok(new { total, page, size, items });
    }

    // ── GET /api/projects/{projectId}/worklogs/summary ─────────────────────────

    /// <summary>
    /// Loyiha bo'yicha ish jurnali xulasasi
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(Guid projectId, CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var summary = await db.WorkLogs
            .Where(w => w.ProjectId == projectId)
            .GroupBy(w => w.BrigadeId)
            .Select(g => new
            {
                BrigadeId   = g.Key,
                BrigadeName = g.First().Brigade!.Name,
                TotalAmount = g.Sum(w => w.TotalAmount),
                PaidAmount  = g.Where(w => w.Status == WorkLogStatus.Paid).Sum(w => w.TotalAmount),
                Confirmed   = g.Count(w => w.Status == WorkLogStatus.Confirmed),
                Draft       = g.Count(w => w.Status == WorkLogStatus.Draft),
            })
            .ToListAsync(ct);

        return Ok(summary);
    }

    // ── POST /api/projects/{projectId}/worklogs ────────────────────────────────

    /// <summary>
    /// Yangi ish jurnali yozuvi qo'shish
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Brigadir")]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateWorkLogRequest req,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        // EstimateItem narxini olish
        var estimateItem = await db.EstimateItems
            .Where(i => i.Id == req.EstimateItemId)
            .Select(i => new { i.BrigadeUnitPrice })
            .FirstOrDefaultAsync(ct);

        if (estimateItem is null)
            return BadRequest(new { message = "Smeta qatori topilmadi" });

        var unitPrice   = req.BrigadeUnitPrice ?? estimateItem.BrigadeUnitPrice;
        var totalAmount = req.Volume * unitPrice;
        var now         = DateTimeOffset.UtcNow;
        var userId      = ctx.UserId ?? Guid.Empty;

        var workLog = new WorkLog
        {
            Id               = Guid.NewGuid(),
            ProjectId        = projectId,
            BrigadeId        = req.BrigadeId,
            EstimateItemId   = req.EstimateItemId,
            Date             = req.Date,
            Volume           = req.Volume,
            BrigadeUnitPrice = unitPrice,
            TotalAmount      = totalAmount,
            Note             = req.Note,
            Status           = WorkLogStatus.Draft,
            CreatedAt        = now, UpdatedAt = now,
            CreatedBy        = userId, UpdatedBy = userId,
        };

        await db.WorkLogs.AddAsync(workLog, ct);
        await db.SaveChangesAsync(ct);

        return Ok(new { workLog.Id, workLog.TotalAmount });
    }

    // ── PUT /api/projects/{projectId}/worklogs/{id} ────────────────────────────

    /// <summary>
    /// Ish jurnali yozuvini yangilash (faqat Draft holatida)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager,Brigadir")]
    public async Task<IActionResult> Update(
        Guid projectId,
        Guid id,
        [FromBody] UpdateWorkLogRequest req,
        CancellationToken ct)
    {
        var workLog = await db.WorkLogs
            .FirstOrDefaultAsync(w => w.Id == id && w.ProjectId == projectId, ct);

        if (workLog is null) return NotFound();

        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (!CanAccess(orgId!.Value)) return Forbid();

        if (workLog.Status != WorkLogStatus.Draft)
            return BadRequest(new { message = "Faqat Draft holatidagi yozuvni o'zgartirish mumkin" });

        workLog.Date             = req.Date;
        workLog.Volume           = req.Volume;
        workLog.BrigadeUnitPrice = req.BrigadeUnitPrice;
        workLog.TotalAmount      = req.Volume * req.BrigadeUnitPrice;
        workLog.Note             = req.Note;
        workLog.UpdatedAt        = DateTimeOffset.UtcNow;
        workLog.UpdatedBy        = ctx.UserId ?? Guid.Empty;

        db.WorkLogs.Update(workLog);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── POST /api/projects/{projectId}/worklogs/confirm ───────────────────────

    /// <summary>
    /// Ish jurnali yozuvlarini tasdiqlash (Draft → Confirmed)
    /// </summary>
    [HttpPost("confirm")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Confirm(
        Guid projectId,
        [FromBody] ConfirmWorkLogRequest req,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var workLogs = await db.WorkLogs
            .Where(w => req.WorkLogIds.Contains(w.Id)
                     && w.ProjectId == projectId
                     && w.Status == WorkLogStatus.Draft)
            .ToListAsync(ct);

        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        foreach (var wl in workLogs)
        {
            wl.Status    = WorkLogStatus.Confirmed;
            wl.UpdatedAt = now;
            wl.UpdatedBy = userId;
            db.WorkLogs.Update(wl);
        }

        await db.SaveChangesAsync(ct);

        return Ok(new { confirmed = workLogs.Count });
    }

    // ── POST /api/projects/{projectId}/worklogs/reject ───────────────────────

    /// <summary>
    /// Ish jurnali yozuvlarini rad etish (Draft/Confirmed → Rejected)
    /// </summary>
    [HttpPost("reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reject(
        Guid projectId,
        [FromBody] RejectWorkLogRequest req,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var workLogs = await db.WorkLogs
            .Where(w => req.WorkLogIds.Contains(w.Id)
                     && w.ProjectId == projectId
                     && (w.Status == WorkLogStatus.Draft || w.Status == WorkLogStatus.Confirmed))
            .ToListAsync(ct);

        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        foreach (var wl in workLogs)
        {
            wl.Status    = WorkLogStatus.Rejected;
            wl.Note      = req.Reason ?? wl.Note;
            wl.UpdatedAt = now;
            wl.UpdatedBy = userId;
            db.WorkLogs.Update(wl);
        }

        await db.SaveChangesAsync(ct);

        return Ok(new { rejected = workLogs.Count });
    }

    // ── DELETE /api/projects/{projectId}/worklogs/{id} ────────────────────────

    /// <summary>
    /// Ish jurnali yozuvini o'chirish (faqat Draft holatida)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id, CancellationToken ct)
    {
        var workLog = await db.WorkLogs
            .FirstOrDefaultAsync(w => w.Id == id && w.ProjectId == projectId, ct);

        if (workLog is null) return NotFound();

        if (workLog.Status != WorkLogStatus.Draft)
            return BadRequest(new { message = "Faqat Draft holatidagi yozuvni o'chirish mumkin" });

        db.WorkLogs.Remove(workLog);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }
}
