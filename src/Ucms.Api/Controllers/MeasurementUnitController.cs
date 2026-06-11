namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

/// <summary>
/// O'lchov birliklari — spravochnik (barcha foydalanuvchilar o'qiydi, Admin yozadi)
/// </summary>
[ApiController]
[Route("api/measurement-units")]
[Tags("Lookup")]
[Authorize]
public class MeasurementUnitController(IUcmsDbContext db) : ControllerBase
{
    public record CreateUnitRequest(
        string Code,
        string Name,
        string NameRu,
        string? NameEn,
        MeasurementUnitType Type,
        decimal Multiplier = 1);

    // ── GET /api/measurement-units ─────────────────────────────────────────────

    /// <summary>
    /// Barcha o'lchov birliklari
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] MeasurementUnitType? type,
        CancellationToken ct)
    {
        var query = db.MeasurementUnits.Where(u => !u.IsDeleted);
        if (type.HasValue) query = query.Where(u => u.Type == type.Value);

        var list = await query
            .OrderBy(u => u.Name)
            .Select(u => new { u.Id, u.Code, u.Name, u.NameRu, u.NameEn, u.Type, u.Multiplier })
            .ToListAsync(ct);

        return Ok(list);
    }

    // ── POST /api/measurement-units ────────────────────────────────────────────

    /// <summary>
    /// Yangi o'lchov birligi qo'shish (faqat Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUnitRequest req, CancellationToken ct)
    {
        if (await db.MeasurementUnits.AnyAsync(u => u.Code == req.Code && !u.IsDeleted, ct))
            return BadRequest(new { message = $"'{req.Code}' kodi allaqachon mavjud" });

        var unit = new MeasurementUnit
        {
            Id         = Guid.NewGuid(),
            Code       = req.Code,
            Name       = req.Name,
            NameRu     = req.NameRu,
            NameEn     = req.NameEn,
            Type       = req.Type,
            Multiplier = req.Multiplier,
            IsDeleted  = false,
        };

        await db.MeasurementUnits.AddAsync(unit, ct);
        await db.SaveChangesAsync(ct);
        return Ok(new { unit.Id, unit.Code, unit.Name });
    }

    // ── DELETE /api/measurement-units/{id} ─────────────────────────────────────

    /// <summary>
    /// O'lchov birligini o'chirish — soft delete (faqat Admin)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var unit = await db.MeasurementUnits.FindAsync([id], ct);
        if (unit is null || unit.IsDeleted) return NotFound();

        unit.IsDeleted = true;
        db.MeasurementUnits.Update(unit);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
