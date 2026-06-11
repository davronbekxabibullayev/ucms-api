namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/projects")]
[Tags("Project")]
[Authorize]
public class ProjectController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record CreateProjectRequest(
        string Name,
        string? Address,
        string? Description,
        string? ContractNumber,
        DateTimeOffset? ContractDate,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate);

    public record UpdateProjectRequest(
        string Name,
        string? Address,
        string? Description,
        string? ContractNumber,
        DateTimeOffset? ContractDate,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        ProjectStatus Status);

    // ── GET /api/projects ──────────────────────────────────────────────────────

    /// <summary>
    /// Loyihalar ro'yxati (tashkilot bo'yicha filtrlangan)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ProjectStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        var orgId = GetOrgId();
        if (orgId is null && !ctx.IsOwner)
            return Forbid();

        var query = db.Projects.Where(p => !p.IsDeleted);

        if (orgId.HasValue)
            query = query.Where(p => p.OrganizationId == orgId.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new
            {
                p.Id, p.Name, p.Address, p.ContractNumber, p.Status,
                p.StartDate, p.EndDate, p.OrganizationId,
                p.CreatedAt,
            })
            .ToListAsync(ct);

        return Ok(new { total, page, size, items });
    }

    // ── GET /api/projects/{id} ─────────────────────────────────────────────────

    /// <summary>
    /// Loyiha tafsilotlari (smeta bo'limlari bilan)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var project = await db.Projects
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new
            {
                p.Id, p.Name, p.Address, p.Description,
                p.ContractNumber, p.ContractDate,
                p.StartDate, p.EndDate, p.Status,
                p.OrganizationId,
                p.CreatedAt, p.UpdatedAt,
                Sections = p.EstimateSections
                    .OrderBy(s => s.Order)
                    .Select(s => new
                    {
                        s.Id, s.Name, s.Order,
                        Items = s.EstimateItems
                            .OrderBy(i => i.Order)
                            .Select(i => new
                            {
                                i.Id, i.Name, i.Unit, i.Volume,
                                i.ClientUnitPrice, i.BrigadeUnitPrice,
                                ClientTotal = i.Volume * i.ClientUnitPrice,
                                BrigadeTotal = i.Volume * i.BrigadeUnitPrice,
                                i.Order,
                            })
                    }),
            })
            .FirstOrDefaultAsync(ct);

        if (project is null) return NotFound();
        if (!CanAccess(project.OrganizationId)) return Forbid();

        return Ok(project);
    }

    // ── POST /api/projects ─────────────────────────────────────────────────────

    /// <summary>
    /// Yangi loyiha yaratish
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest req, CancellationToken ct)
    {
        var orgId = GetOrgId();
        if (!orgId.HasValue)
            return BadRequest(new { message = "Foydalanuvchiga tashkilot biriktirilmagan" });

        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        var project = new Project
        {
            Id             = Guid.NewGuid(),
            OrganizationId = orgId.Value,
            Name           = req.Name,
            Address        = req.Address,
            Description    = req.Description,
            ContractNumber = req.ContractNumber,
            ContractDate   = req.ContractDate,
            StartDate      = req.StartDate,
            EndDate        = req.EndDate,
            Status         = ProjectStatus.Planning,
            IsDeleted      = false,
            CreatedAt      = now, UpdatedAt = now,
            CreatedBy      = userId, UpdatedBy = userId,
        };

        await db.Projects.AddAsync(project, ct);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, new { project.Id, project.Name });
    }

    // ── PUT /api/projects/{id} ─────────────────────────────────────────────────

    /// <summary>
    /// Loyiha ma'lumotlarini yangilash
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest req, CancellationToken ct)
    {
        var project = await db.Projects.FindAsync([id], ct);
        if (project is null || project.IsDeleted) return NotFound();
        if (!CanAccess(project.OrganizationId)) return Forbid();

        project.Name           = req.Name;
        project.Address        = req.Address;
        project.Description    = req.Description;
        project.ContractNumber = req.ContractNumber;
        project.ContractDate   = req.ContractDate;
        project.StartDate      = req.StartDate;
        project.EndDate        = req.EndDate;
        project.Status         = req.Status;
        project.UpdatedAt      = DateTimeOffset.UtcNow;
        project.UpdatedBy      = ctx.UserId ?? Guid.Empty;

        db.Projects.Update(project);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── DELETE /api/projects/{id} ─────────────────────────────────────────────

    /// <summary>
    /// Loyihani o'chirish (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var project = await db.Projects.FindAsync([id], ct);
        if (project is null || project.IsDeleted) return NotFound();
        if (!CanAccess(project.OrganizationId)) return Forbid();

        project.IsDeleted = true;
        project.UpdatedAt = DateTimeOffset.UtcNow;
        project.UpdatedBy = ctx.UserId ?? Guid.Empty;

        db.Projects.Update(project);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    // Owner foydalanuvchilar barcha tashkilotlar loyihalarini ko'radi va boshqaradi
    private Guid? GetOrgId() =>
        ctx.IsOwner ? null : ctx.OrganizationId;

    private bool CanAccess(Guid orgId) =>
        ctx.IsOwner || ctx.OrganizationId == orgId;
}
