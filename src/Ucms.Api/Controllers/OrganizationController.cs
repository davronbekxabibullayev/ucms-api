namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/organizations")]
[Tags("Organization")]
[Authorize]
public class OrganizationController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record CreateOrganizationRequest(
        string Name,
        string? TaxId,
        string? Address,
        string? Phone,
        string? Email,
        OrganizationType Type = OrganizationType.Tenant);

    public record UpdateOrganizationRequest(
        string Name,
        string? TaxId,
        string? Address,
        string? Phone,
        string? Email);

    // ── GET /api/organizations ─────────────────────────────────────────────────

    /// <summary>
    /// Barcha tashkilotlar ro'yxati (Admin uchun — hammasi, boshqalar — o'ziniki)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var query = db.Organizations.Where(o => !o.IsDeleted);

        // Owner foydalanuvchilar barcha tashkilotlarni ko'radi; Tenant — faqat o'ziniki
        if (!ctx.IsOwner && ctx.OrganizationId.HasValue)
            query = query.Where(o => o.Id == ctx.OrganizationId.Value);

        var list = await query
            .OrderBy(o => o.Name)
            .Select(o => new
            {
                o.Id, o.Name, o.TaxId, o.Address, o.Phone, o.Email,
                o.Type, o.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    // ── GET /api/organizations/{id} ────────────────────────────────────────────

    /// <summary>
    /// Tashkilot ma'lumotlari
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        if (!CanAccess(id))
            return Forbid();

        var org = await db.Organizations
            .Where(o => o.Id == id && !o.IsDeleted)
            .Select(o => new
            {
                o.Id, o.Name, o.TaxId, o.Address, o.Phone, o.Email,
                o.Type, o.CreatedAt, o.UpdatedAt,
                ProjectCount = o.Projects.Count(p => !p.IsDeleted),
                BrigadeCount = o.Brigades.Count(b => !b.IsDeleted),
            })
            .FirstOrDefaultAsync(ct);

        return org is null ? NotFound() : Ok(org);
    }

    // ── POST /api/organizations ────────────────────────────────────────────────

    /// <summary>
    /// Yangi tashkilot yaratish (faqat Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest req, CancellationToken ct)
    {
        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        var org = new Organization
        {
            Id        = Guid.NewGuid(),
            Name      = req.Name,
            TaxId     = req.TaxId,
            Address   = req.Address,
            Phone     = req.Phone,
            Email     = req.Email,
            Type      = req.Type,
            IsDeleted = false,
            CreatedAt = now, UpdatedAt = now,
            CreatedBy = userId, UpdatedBy = userId,
        };

        await db.Organizations.AddAsync(org, ct);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = org.Id }, new { org.Id, org.Name });
    }

    // ── PUT /api/organizations/{id} ────────────────────────────────────────────

    /// <summary>
    /// Tashkilot ma'lumotlarini yangilash
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationRequest req, CancellationToken ct)
    {
        if (!CanAccess(id))
            return Forbid();

        var org = await db.Organizations.FindAsync([id], ct);
        if (org is null || org.IsDeleted)
            return NotFound();

        org.Name      = req.Name;
        org.TaxId     = req.TaxId;
        org.Address   = req.Address;
        org.Phone     = req.Phone;
        org.Email     = req.Email;
        org.UpdatedAt = DateTimeOffset.UtcNow;
        org.UpdatedBy = ctx.UserId ?? Guid.Empty;

        db.Organizations.Update(org);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── DELETE /api/organizations/{id} ────────────────────────────────────────

    /// <summary>
    /// Tashkilotni o'chirish (soft delete, faqat Admin)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var org = await db.Organizations.FindAsync([id], ct);
        if (org is null || org.IsDeleted)
            return NotFound();

        org.IsDeleted = true;
        org.UpdatedAt = DateTimeOffset.UtcNow;
        org.UpdatedBy = ctx.UserId ?? Guid.Empty;

        db.Organizations.Update(org);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private bool CanAccess(Guid orgId) =>
        ctx.IsOwner || ctx.OrganizationId == orgId;
}
