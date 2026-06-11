namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;

[ApiController]
[Route("api/brigades")]
[Tags("Brigade")]
[Authorize]
public class BrigadeController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record CreateBrigadeRequest(
        string Name,
        string? ForemanName,
        string? Phone);

    public record UpdateBrigadeRequest(
        string Name,
        string? ForemanName,
        string? Phone,
        bool IsActive);

    // ── GET /api/brigades ──────────────────────────────────────────────────────

    /// <summary>
    /// Brigadalar ro'yxati
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive,
        CancellationToken ct)
    {
        var query = db.Brigades.Where(b => !b.IsDeleted);

        if (!ctx.IsAdmin && ctx.OrganizationId.HasValue)
            query = query.Where(b => b.OrganizationId == ctx.OrganizationId.Value);

        if (isActive.HasValue)
            query = query.Where(b => b.IsActive == isActive.Value);

        var list = await query
            .OrderBy(b => b.Name)
            .Select(b => new
            {
                b.Id, b.Name, b.ForemanName, b.Phone, b.IsActive,
                b.OrganizationId, b.CreatedAt,
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    // ── GET /api/brigades/{id} ─────────────────────────────────────────────────

    /// <summary>
    /// Brigada ma'lumotlari
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var brigade = await db.Brigades
            .Where(b => b.Id == id && !b.IsDeleted)
            .Select(b => new
            {
                b.Id, b.Name, b.ForemanName, b.Phone, b.IsActive,
                b.OrganizationId, b.CreatedAt, b.UpdatedAt,
            })
            .FirstOrDefaultAsync(ct);

        if (brigade is null) return NotFound();
        if (!CanAccess(brigade.OrganizationId)) return Forbid();

        return Ok(brigade);
    }

    // ── POST /api/brigades ─────────────────────────────────────────────────────

    /// <summary>
    /// Yangi brigada yaratish
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateBrigadeRequest req, CancellationToken ct)
    {
        var orgId = ctx.OrganizationId;
        if (!orgId.HasValue)
            return BadRequest(new { message = "Foydalanuvchiga tashkilot biriktirilmagan" });

        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        var brigade = new Brigade
        {
            Id             = Guid.NewGuid(),
            OrganizationId = orgId.Value,
            Name           = req.Name,
            ForemanName    = req.ForemanName,
            Phone          = req.Phone,
            IsActive       = true,
            IsDeleted      = false,
            CreatedAt      = now, UpdatedAt = now,
            CreatedBy      = userId, UpdatedBy = userId,
        };

        await db.Brigades.AddAsync(brigade, ct);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = brigade.Id }, new { brigade.Id, brigade.Name });
    }

    // ── PUT /api/brigades/{id} ─────────────────────────────────────────────────

    /// <summary>
    /// Brigada ma'lumotlarini yangilash
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrigadeRequest req, CancellationToken ct)
    {
        var brigade = await db.Brigades.FindAsync([id], ct);
        if (brigade is null || brigade.IsDeleted) return NotFound();
        if (!CanAccess(brigade.OrganizationId)) return Forbid();

        brigade.Name        = req.Name;
        brigade.ForemanName = req.ForemanName;
        brigade.Phone       = req.Phone;
        brigade.IsActive    = req.IsActive;
        brigade.UpdatedAt   = DateTimeOffset.UtcNow;
        brigade.UpdatedBy   = ctx.UserId ?? Guid.Empty;

        db.Brigades.Update(brigade);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── DELETE /api/brigades/{id} ─────────────────────────────────────────────

    /// <summary>
    /// Brigadani o'chirish (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var brigade = await db.Brigades.FindAsync([id], ct);
        if (brigade is null || brigade.IsDeleted) return NotFound();
        if (!CanAccess(brigade.OrganizationId)) return Forbid();

        brigade.IsDeleted = true;
        brigade.IsActive  = false;
        brigade.UpdatedAt = DateTimeOffset.UtcNow;
        brigade.UpdatedBy = ctx.UserId ?? Guid.Empty;

        db.Brigades.Update(brigade);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private bool CanAccess(Guid orgId) =>
        ctx.IsAdmin || ctx.OrganizationId == orgId;
}
