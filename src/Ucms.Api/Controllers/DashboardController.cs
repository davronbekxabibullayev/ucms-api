namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;

/// <summary>
/// Bosh sahifa statistikasi
/// </summary>
[ApiController]
[Route("api/dashboard")]
[Tags("Dashboard")]
[Authorize]
public class DashboardController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── GET /api/dashboard ─────────────────────────────────────────────────────

    /// <summary>
    /// Tashkilot bo'yicha umumiy ko'rsatkichlar
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var orgId = ctx.OrganizationId;

        // Loyihalar statistikasi
        var projectsQuery = db.Projects.Where(p => !p.IsDeleted);
        if (orgId.HasValue) projectsQuery = projectsQuery.Where(p => p.OrganizationId == orgId.Value);

        var projectStats = await projectsQuery
            .GroupBy(_ => true)
            .Select(g => new
            {
                Total      = g.Count(),
                Planning   = g.Count(p => p.Status == ProjectStatus.Planning),
                InProgress = g.Count(p => p.Status == ProjectStatus.InProgress),
                Completed  = g.Count(p => p.Status == ProjectStatus.Completed),
                Suspended  = g.Count(p => p.Status == ProjectStatus.Suspended),
            })
            .FirstOrDefaultAsync(ct)
            ?? new { Total = 0, Planning = 0, InProgress = 0, Completed = 0, Suspended = 0 };

        // Brigadalar statistikasi
        var brigadesQuery = db.Brigades.Where(b => !b.IsDeleted);
        if (orgId.HasValue) brigadesQuery = brigadesQuery.Where(b => b.OrganizationId == orgId.Value);

        var brigadeStats = await brigadesQuery
            .GroupBy(_ => true)
            .Select(g => new { Total = g.Count(), Active = g.Count(b => b.IsActive) })
            .FirstOrDefaultAsync(ct)
            ?? new { Total = 0, Active = 0 };

        // Moliyaviy ko'rsatkichlar (faol loyihalar bo'yicha)
        var activeProjectIds = await projectsQuery
            .Where(p => p.Status == ProjectStatus.InProgress)
            .Select(p => p.Id)
            .ToListAsync(ct);

        decimal clientReceived = 0, brigadePaid = 0, workedTotal = 0;

        if (activeProjectIds.Count != 0)
        {
            clientReceived = await db.ClientPayments
                .Where(p => activeProjectIds.Contains(p.ProjectId))
                .SumAsync(p => p.Amount, ct);

            brigadePaid = await db.BrigadePayments
                .Where(p => activeProjectIds.Contains(p.ProjectId))
                .SumAsync(p => p.Amount, ct);

            workedTotal = await db.WorkLogs
                .Where(w => activeProjectIds.Contains(w.ProjectId))
                .SumAsync(w => w.TotalAmount, ct);
        }

        // So'nggi 5 ta ish jurnali yozuvi
        var recentWorkLogs = await db.WorkLogs
            .Where(w => !orgId.HasValue || activeProjectIds.Contains(w.ProjectId))
            .OrderByDescending(w => w.CreatedAt)
            .Take(5)
            .Select(w => new
            {
                w.Id, w.Date, w.TotalAmount, w.Status,
                Project      = w.Project!.Name,
                Brigade      = w.Brigade!.Name,
                EstimateItem = w.EstimateItem!.Name,
            })
            .ToListAsync(ct);

        // So'nggi 5 ta to'lov
        var recentPayments = await db.ClientPayments
            .Where(p => !orgId.HasValue || activeProjectIds.Contains(p.ProjectId))
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new
            {
                p.Id, p.Date, p.Amount, p.PaymentMethod,
                Project   = p.Project!.Name,
                ActNumber = p.Act != null ? p.Act.ActNumber : null,
            })
            .ToListAsync(ct);

        return Ok(new
        {
            Projects = projectStats,
            Brigades = brigadeStats,
            Finance  = new
            {
                ClientReceived = clientReceived,
                BrigadePaid    = brigadePaid,
                WorkedTotal    = workedTotal,
                BrigadeDebt    = workedTotal - brigadePaid,
            },
            RecentWorkLogs = recentWorkLogs,
            RecentPayments = recentPayments,
        });
    }

    // ── GET /api/dashboard/projects/{projectId} ────────────────────────────────

    /// <summary>
    /// Bitta loyiha bo'yicha batafsil moliyaviy ko'rsatkich
    /// </summary>
    [HttpGet("projects/{projectId:guid}")]
    public async Task<IActionResult> GetProjectDetail(Guid projectId, CancellationToken ct)
    {
        var project = await db.Projects
            .Where(p => p.Id == projectId && !p.IsDeleted)
            .Select(p => new { p.Name, p.Status, p.OrganizationId })
            .FirstOrDefaultAsync(ct);

        if (project is null) return NotFound();

        if (!ctx.IsOwner && ctx.OrganizationId != project.OrganizationId)
            return Forbid();

        // Smeta jami
        var estimateTotals = await db.EstimateItems
            .Where(i => i.Section!.ProjectId == projectId)
            .GroupBy(_ => true)
            .Select(g => new
            {
                ClientTotal  = g.Sum(i => i.Volume * i.ClientUnitPrice),
                BrigadeTotal = g.Sum(i => i.Volume * i.BrigadeUnitPrice),
            })
            .FirstOrDefaultAsync(ct)
            ?? new { ClientTotal = 0m, BrigadeTotal = 0m };

        // Bajarilgan ishlar
        var workStats = await db.WorkLogs
            .Where(w => w.ProjectId == projectId)
            .GroupBy(_ => true)
            .Select(g => new
            {
                Worked    = g.Sum(w => w.TotalAmount),
                Confirmed = g.Where(w => w.Status == WorkLogStatus.Confirmed).Sum(w => w.TotalAmount),
                Paid      = g.Where(w => w.Status == WorkLogStatus.Paid).Sum(w => w.TotalAmount),
            })
            .FirstOrDefaultAsync(ct)
            ?? new { Worked = 0m, Confirmed = 0m, Paid = 0m };

        // Aktlar
        var actStats = await db.ClientActs
            .Where(a => a.ProjectId == projectId)
            .GroupBy(_ => true)
            .Select(g => new
            {
                Total       = g.Count(),
                TotalAmount = g.Sum(a => a.TotalAmount),
                Issued      = g.Count(a => a.Status == ActStatus.Issued),
                PaidFully   = g.Count(a => a.Status == ActStatus.PaidFully),
            })
            .FirstOrDefaultAsync(ct)
            ?? new { Total = 0, TotalAmount = 0m, Issued = 0, PaidFully = 0 };

        // To'lovlar
        var clientReceived = await db.ClientPayments
            .Where(p => p.ProjectId == projectId).SumAsync(p => p.Amount, ct);

        var brigadePaid = await db.BrigadePayments
            .Where(p => p.ProjectId == projectId).SumAsync(p => p.Amount, ct);

        // Brigadalar bo'yicha ish holati
        var brigadeBreakdown = await db.WorkLogs
            .Where(w => w.ProjectId == projectId)
            .GroupBy(w => w.BrigadeId)
            .Select(g => new
            {
                BrigadeId   = g.Key,
                BrigadeName = g.First().Brigade!.Name,
                TotalWorked = g.Sum(w => w.TotalAmount),
                TotalPaid   = g.Where(w => w.Status == WorkLogStatus.Paid).Sum(w => w.TotalAmount),
                Debt        = g.Where(w => w.Status != WorkLogStatus.Paid).Sum(w => w.TotalAmount),
            })
            .ToListAsync(ct);

        return Ok(new
        {
            Project = new { project.Name, project.Status },
            Estimate = new
            {
                estimateTotals.ClientTotal,
                estimateTotals.BrigadeTotal,
                Profit = estimateTotals.ClientTotal - estimateTotals.BrigadeTotal,
            },
            Work = workStats,
            Acts = actStats,
            Finance = new
            {
                ClientTotal    = estimateTotals.ClientTotal,
                ClientReceived = clientReceived,
                ClientDebt     = estimateTotals.ClientTotal - clientReceived,
                BrigadeTotal   = workStats.Worked,
                BrigadePaid    = brigadePaid,
                BrigadeDebt    = workStats.Worked - brigadePaid,
                NetBalance     = clientReceived - brigadePaid,
            },
            BrigadeBreakdown = brigadeBreakdown,
        });
    }
}
