namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;

/// <summary>
/// Smeta bo'limlari va qatorlarini boshqarish
/// </summary>
[ApiController]
[Route("api/projects/{projectId:guid}/estimate")]
[Tags("Estimate")]
[Authorize(Roles = "Admin,Manager")]
public class EstimateController(
    IUcmsDbContext db,
    ICurrentContext ctx) : ControllerBase
{
    // ── Section requests ───────────────────────────────────────────────────────

    public record CreateSectionRequest(string Name, int Order);
    public record UpdateSectionRequest(string Name, int Order);

    // ── Item requests ──────────────────────────────────────────────────────────

    public record CreateItemRequest(
        Guid SectionId,
        string Name,
        string Unit,
        decimal Volume,
        decimal ClientUnitPrice,
        decimal BrigadeUnitPrice,
        int Order);

    public record UpdateItemRequest(
        string Name,
        string Unit,
        decimal Volume,
        decimal ClientUnitPrice,
        decimal BrigadeUnitPrice,
        int Order);

    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task<bool> ProjectBelongsToOrgAsync(Guid projectId, CancellationToken ct)
    {
        var orgId = await db.Projects
            .Where(p => p.Id == projectId && !p.IsDeleted)
            .Select(p => (Guid?)p.OrganizationId)
            .FirstOrDefaultAsync(ct);

        if (orgId is null) return false;
        return ctx.IsAdmin || ctx.OrganizationId == orgId;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // SECTIONS
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Loyiha smeta bo'limlari ro'yxati
    /// </summary>
    [HttpGet("sections")]
    [Authorize(Roles = "Admin,Manager,Brigadir,Accountant")]
    public async Task<IActionResult> GetSections(Guid projectId, CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        var sections = await db.EstimateSections
            .Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.Order)
            .Select(s => new
            {
                s.Id, s.Name, s.Order,
                ItemCount = s.EstimateItems.Count(),
                ClientTotal = s.EstimateItems.Sum(i => i.Volume * i.ClientUnitPrice),
                BrigadeTotal = s.EstimateItems.Sum(i => i.Volume * i.BrigadeUnitPrice),
            })
            .ToListAsync(ct);

        return Ok(sections);
    }

    /// <summary>
    /// Yangi smeta bo'limi qo'shish
    /// </summary>
    [HttpPost("sections")]
    public async Task<IActionResult> CreateSection(
        Guid projectId,
        [FromBody] CreateSectionRequest req,
        CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        var section = new EstimateSection
        {
            Id        = Guid.NewGuid(),
            ProjectId = projectId,
            Name      = req.Name,
            Order     = req.Order,
        };

        await db.EstimateSections.AddAsync(section, ct);
        await db.SaveChangesAsync(ct);

        return Ok(new { section.Id, section.Name, section.Order });
    }

    /// <summary>
    /// Smeta bo'limini yangilash
    /// </summary>
    [HttpPut("sections/{sectionId:guid}")]
    public async Task<IActionResult> UpdateSection(
        Guid projectId,
        Guid sectionId,
        [FromBody] UpdateSectionRequest req,
        CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        var section = await db.EstimateSections
            .FirstOrDefaultAsync(s => s.Id == sectionId && s.ProjectId == projectId, ct);

        if (section is null) return NotFound();

        section.Name  = req.Name;
        section.Order = req.Order;

        db.EstimateSections.Update(section);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    /// <summary>
    /// Smeta bo'limini o'chirish
    /// </summary>
    [HttpDelete("sections/{sectionId:guid}")]
    public async Task<IActionResult> DeleteSection(
        Guid projectId,
        Guid sectionId,
        CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        var section = await db.EstimateSections
            .FirstOrDefaultAsync(s => s.Id == sectionId && s.ProjectId == projectId, ct);

        if (section is null) return NotFound();

        db.EstimateSections.Remove(section);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // ITEMS
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Smeta bo'limi qatorlari
    /// </summary>
    [HttpGet("sections/{sectionId:guid}/items")]
    [Authorize(Roles = "Admin,Manager,Brigadir,Accountant")]
    public async Task<IActionResult> GetItems(
        Guid projectId,
        Guid sectionId,
        CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        var items = await db.EstimateItems
            .Where(i => i.SectionId == sectionId)
            .OrderBy(i => i.Order)
            .Select(i => new
            {
                i.Id, i.Name, i.Unit, i.Volume,
                i.ClientUnitPrice, i.BrigadeUnitPrice,
                ClientTotal  = i.Volume * i.ClientUnitPrice,
                BrigadeTotal = i.Volume * i.BrigadeUnitPrice,
                i.Order,
            })
            .ToListAsync(ct);

        return Ok(items);
    }

    /// <summary>
    /// Yangi smeta qatori qo'shish
    /// </summary>
    [HttpPost("items")]
    public async Task<IActionResult> CreateItem(
        Guid projectId,
        [FromBody] CreateItemRequest req,
        CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        // SectionId loyihaga tegishli ekanligini tekshirish
        var sectionExists = await db.EstimateSections
            .AnyAsync(s => s.Id == req.SectionId && s.ProjectId == projectId, ct);

        if (!sectionExists)
            return BadRequest(new { message = "Bo'lim ushbu loyihaga tegishli emas" });

        var item = new EstimateItem
        {
            Id               = Guid.NewGuid(),
            SectionId        = req.SectionId,
            Name             = req.Name,
            Unit             = req.Unit,
            Volume           = req.Volume,
            ClientUnitPrice  = req.ClientUnitPrice,
            BrigadeUnitPrice = req.BrigadeUnitPrice,
            Order            = req.Order,
        };

        await db.EstimateItems.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);

        return Ok(new { item.Id, item.Name });
    }

    /// <summary>
    /// Smeta qatorini yangilash
    /// </summary>
    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid projectId,
        Guid itemId,
        [FromBody] UpdateItemRequest req,
        CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        var item = await db.EstimateItems
            .Include(i => i.Section)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.Section!.ProjectId == projectId, ct);

        if (item is null) return NotFound();

        item.Name             = req.Name;
        item.Unit             = req.Unit;
        item.Volume           = req.Volume;
        item.ClientUnitPrice  = req.ClientUnitPrice;
        item.BrigadeUnitPrice = req.BrigadeUnitPrice;
        item.Order            = req.Order;

        db.EstimateItems.Update(item);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    /// <summary>
    /// Smeta qatorini o'chirish
    /// </summary>
    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> DeleteItem(
        Guid projectId,
        Guid itemId,
        CancellationToken ct)
    {
        if (!await ProjectBelongsToOrgAsync(projectId, ct))
            return Forbid();

        var item = await db.EstimateItems
            .Include(i => i.Section)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.Section!.ProjectId == projectId, ct);

        if (item is null) return NotFound();

        db.EstimateItems.Remove(item);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }
}
