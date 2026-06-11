namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/projects/{projectId:guid}/payments")]
[Tags("Payment")]
[Authorize]
public class PaymentController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record CreateClientPaymentRequest(
        Guid? ActId,
        DateTimeOffset Date,
        decimal Amount,
        PaymentMethod PaymentMethod,
        string? Note);

    public record CreateBrigadePaymentRequest(
        Guid BrigadeId,
        DateTimeOffset Date,
        decimal Amount,
        PaymentMethod PaymentMethod,
        Guid[] WorkLogIds,
        string? Note);

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task<Guid?> GetProjectOrgAsync(Guid projectId, CancellationToken ct) =>
        await db.Projects
            .Where(p => p.Id == projectId && !p.IsDeleted)
            .Select(p => (Guid?)p.OrganizationId)
            .FirstOrDefaultAsync(ct);

    private bool CanAccess(Guid orgId) =>
        ctx.IsAdmin || ctx.OrganizationId == orgId;

    // ══════════════════════════════════════════════════════════════════════════
    // CLIENT PAYMENTS (Zakazchik to'lovlari)
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Zakazchik to'lovlari ro'yxati
    /// </summary>
    [HttpGet("client")]
    public async Task<IActionResult> GetClientPayments(
        Guid projectId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken ct = default)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var query = db.ClientPayments.Where(p => p.ProjectId == projectId);
        if (from.HasValue) query = query.Where(p => p.Date >= from.Value);
        if (to.HasValue)   query = query.Where(p => p.Date <= to.Value);

        var list = await query
            .OrderByDescending(p => p.Date)
            .Select(p => new
            {
                p.Id, p.Date, p.Amount, p.PaymentMethod, p.Note,
                ActNumber = p.Act != null ? p.Act.ActNumber : null,
            })
            .ToListAsync(ct);

        var total = list.Sum(p => p.Amount);

        return Ok(new { total, items = list });
    }

    /// <summary>
    /// Zakazchik to'lovini kiritish
    /// </summary>
    [HttpPost("client")]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> CreateClientPayment(
        Guid projectId,
        [FromBody] CreateClientPaymentRequest req,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        var payment = new ClientPayment
        {
            Id            = Guid.NewGuid(),
            ProjectId     = projectId,
            ActId         = req.ActId,
            Date          = req.Date,
            Amount        = req.Amount,
            PaymentMethod = req.PaymentMethod,
            Note          = req.Note,
            CreatedAt     = now, UpdatedAt = now,
            CreatedBy     = userId, UpdatedBy = userId,
        };

        await db.ClientPayments.AddAsync(payment, ct);

        // Akt to'lov holatini yangilash
        if (req.ActId.HasValue)
            await UpdateActPaymentStatusAsync(req.ActId.Value, ct);

        await db.SaveChangesAsync(ct);

        return Ok(new { payment.Id, payment.Amount });
    }

    // ══════════════════════════════════════════════════════════════════════════
    // BRIGADE PAYMENTS (Brigada to'lovlari)
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Brigada to'lovlari ro'yxati
    /// </summary>
    [HttpGet("brigade")]
    public async Task<IActionResult> GetBrigadePayments(
        Guid projectId,
        [FromQuery] Guid? brigadeId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken ct = default)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var query = db.BrigadePayments.Where(p => p.ProjectId == projectId);
        if (brigadeId.HasValue) query = query.Where(p => p.BrigadeId == brigadeId.Value);
        if (from.HasValue)      query = query.Where(p => p.Date >= from.Value);
        if (to.HasValue)        query = query.Where(p => p.Date <= to.Value);

        var list = await query
            .OrderByDescending(p => p.Date)
            .Select(p => new
            {
                p.Id, p.Date, p.Amount, p.PaymentMethod, p.Note,
                BrigadeName  = p.Brigade!.Name,
                WorkLogCount = p.WorkLogs.Count,
            })
            .ToListAsync(ct);

        var total = list.Sum(p => p.Amount);

        return Ok(new { total, items = list });
    }

    /// <summary>
    /// Brigadaga to'lov amalga oshirish
    /// </summary>
    [HttpPost("brigade")]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> CreateBrigadePayment(
        Guid projectId,
        [FromBody] CreateBrigadePaymentRequest req,
        CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        var paymentId = Guid.NewGuid();

        var payment = new BrigadePayment
        {
            Id            = paymentId,
            ProjectId     = projectId,
            BrigadeId     = req.BrigadeId,
            Date          = req.Date,
            Amount        = req.Amount,
            PaymentMethod = req.PaymentMethod,
            Note          = req.Note,
            CreatedAt     = now, UpdatedAt = now,
            CreatedBy     = userId, UpdatedBy = userId,
        };

        await db.BrigadePayments.AddAsync(payment, ct);

        // WorkLog larni Paid holatiga o'tkazish va to'lovga bog'lash
        if (req.WorkLogIds.Length > 0)
        {
            var workLogs = await db.WorkLogs
                .Where(w => req.WorkLogIds.Contains(w.Id)
                         && w.ProjectId == projectId
                         && w.BrigadeId == req.BrigadeId
                         && w.Status == WorkLogStatus.Confirmed)
                .ToListAsync(ct);

            foreach (var wl in workLogs)
            {
                wl.Status          = WorkLogStatus.Paid;
                wl.BrigadePaymentId = paymentId;
                wl.UpdatedAt       = now;
                wl.UpdatedBy       = userId;
                db.WorkLogs.Update(wl);
            }
        }

        await db.SaveChangesAsync(ct);

        return Ok(new { payment.Id, payment.Amount });
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PROJECT FINANCIAL SUMMARY
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Loyiha moliyaviy xulosasi (daromad, xarajat, balans)
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetFinancialSummary(Guid projectId, CancellationToken ct)
    {
        var orgId = await GetProjectOrgAsync(projectId, ct);
        if (orgId is null) return NotFound();
        if (!CanAccess(orgId.Value)) return Forbid();

        var clientReceived = await db.ClientPayments
            .Where(p => p.ProjectId == projectId)
            .SumAsync(p => p.Amount, ct);

        var brigadePaid = await db.BrigadePayments
            .Where(p => p.ProjectId == projectId)
            .SumAsync(p => p.Amount, ct);

        var estimateClientTotal = await db.Projects
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.EstimateSections)
            .SelectMany(s => s.EstimateItems)
            .SumAsync(i => i.Volume * i.ClientUnitPrice, ct);

        var estimateBrigadeTotal = await db.Projects
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.EstimateSections)
            .SelectMany(s => s.EstimateItems)
            .SumAsync(i => i.Volume * i.BrigadeUnitPrice, ct);

        var workedBrigadeTotal = await db.WorkLogs
            .Where(w => w.ProjectId == projectId)
            .SumAsync(w => w.TotalAmount, ct);

        return Ok(new
        {
            EstimateClientTotal  = estimateClientTotal,
            EstimateBrigadeTotal = estimateBrigadeTotal,
            EstimateProfit       = estimateClientTotal - estimateBrigadeTotal,

            ClientReceived   = clientReceived,
            ClientDebt       = estimateClientTotal - clientReceived,

            WorkedBrigadeTotal = workedBrigadeTotal,
            BrigadePaid        = brigadePaid,
            BrigadeDebt        = workedBrigadeTotal - brigadePaid,

            NetBalance = clientReceived - brigadePaid,
        });
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task UpdateActPaymentStatusAsync(Guid actId, CancellationToken ct)
    {
        var act = await db.ClientActs
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.Id == actId, ct);

        if (act is null) return;

        var paid = act.Payments.Sum(p => p.Amount);

        act.Status = paid >= act.TotalAmount
            ? ActStatus.PaidFully
            : paid > 0
                ? ActStatus.PaidPartially
                : ActStatus.Issued;

        db.ClientActs.Update(act);
    }
}
