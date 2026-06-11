namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/projects/{projectId:guid}/acts")]
[Tags("ClientAct")]
[Authorize]
public class ClientActController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record ActItemDto(Guid EstimateItemId, decimal Volume, decimal UnitPrice);

    public record CreateActRequest(
        string ActNumber,
        DateTimeOffset ActDate,
        List<ActItemDto> Items,
        string? Note);

    public record UpdateActStatusRequest(ActStatus Status);

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task<Guid?> GetProjectOrgAsync(Guid projectId, CancellationToken ct) =>
        await db.Projects
            .Where(p => p.Id == projectId && !p.IsDeleted)
            .Select(p => (Guid?)p.OrganizationId)
            .FirstOrDefaultAsync(ct);

    private bool CanAccess(Guid orgId) =>
        ctx.IsOwner || ctx.OrganizationId == orgId;

    // ── GET /api/projects/{projectId}/acts ─────────────────────────────────────

    /// <summary>
    /// Loyiha aktlari ro'yxati
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        Guid projectId,
        [FromQuery] ActStatus? status,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var query = db.ClientActs.Where(a => a.ProjectId == projectId);
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);

        var list = await query
            .OrderByDescending(a => a.ActDate)
            .Select(a => new
            {
                a.Id, a.ActNumber, a.ActDate, a.TotalAmount, a.Status, a.Note,
                PaidAmount  = a.Payments.Sum(p => p.Amount),
                ItemCount   = a.Items.Count,
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    // ── GET /api/projects/{projectId}/acts/{id} ────────────────────────────────

    /// <summary>
    /// Akt tafsilotlari (qatorlar va to'lovlar bilan)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid projectId, Guid id, CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var act = await db.ClientActs
            .Where(a => a.Id == id && a.ProjectId == projectId)
            .Select(a => new
            {
                a.Id, a.ActNumber, a.ActDate, a.TotalAmount, a.Status, a.Note,
                a.CreatedAt, a.UpdatedAt,
                Items = a.Items.Select(i => new
                {
                    i.Id, i.EstimateItemId,
                    ItemName  = i.EstimateItem!.Name,
                    Unit      = i.EstimateItem.Unit,
                    i.Volume, i.UnitPrice, i.TotalAmount,
                }),
                Payments = a.Payments.Select(p => new
                {
                    p.Id, p.Date, p.Amount, p.PaymentMethod, p.Note,
                }),
                PaidAmount = a.Payments.Sum(p => p.Amount),
            })
            .FirstOrDefaultAsync(ct);

        return act is null ? NotFound() : Ok(act);
    }

    // ── POST /api/projects/{projectId}/acts ────────────────────────────────────

    /// <summary>
    /// Yangi akt yaratish
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateActRequest req,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var actId      = Guid.NewGuid();
        var totalAmount = req.Items.Sum(i => i.Volume * i.UnitPrice);
        var now         = DateTimeOffset.UtcNow;
        var userId      = ctx.UserId ?? Guid.Empty;

        var act = new ClientAct
        {
            Id          = actId,
            ProjectId   = projectId,
            ActNumber   = req.ActNumber,
            ActDate     = req.ActDate,
            TotalAmount = totalAmount,
            Status      = ActStatus.Draft,
            Note        = req.Note,
            CreatedAt   = now, UpdatedAt = now,
            CreatedBy   = userId, UpdatedBy = userId,
        };

        var items = req.Items.Select(i => new ClientActItem
        {
            Id             = Guid.NewGuid(),
            ActId          = actId,
            EstimateItemId = i.EstimateItemId,
            Volume         = i.Volume,
            UnitPrice      = i.UnitPrice,
            TotalAmount    = i.Volume * i.UnitPrice,
        }).ToList();

        await db.ClientActs.AddAsync(act, ct);
        await db.ClientActItems.AddRangeAsync(items, ct);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { projectId, id = actId },
            new { id = actId, act.ActNumber, act.TotalAmount });
    }

    // ── PATCH /api/projects/{projectId}/acts/{id}/status ──────────────────────

    /// <summary>
    /// Akt holatini yangilash
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> UpdateStatus(
        Guid projectId,
        Guid id,
        [FromBody] UpdateActStatusRequest req,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var act = await db.ClientActs
            .FirstOrDefaultAsync(a => a.Id == id && a.ProjectId == projectId, ct);

        if (act is null) return NotFound();

        act.Status    = req.Status;
        act.UpdatedAt = DateTimeOffset.UtcNow;
        act.UpdatedBy = ctx.UserId ?? Guid.Empty;

        db.ClientActs.Update(act);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── DELETE /api/projects/{projectId}/acts/{id} ────────────────────────────

    /// <summary>
    /// Aktni o'chirish (faqat Draft holatida)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id, CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var act = await db.ClientActs
            .FirstOrDefaultAsync(a => a.Id == id && a.ProjectId == projectId, ct);

        if (act is null) return NotFound();

        if (act.Status != ActStatus.Draft)
            return BadRequest(new { message = "Faqat Draft holatidagi aktni o'chirish mumkin" });

        // Cascade: items va payments ham o'chadi (EF configuration bo'yicha)
        db.ClientActs.Remove(act);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }
}
