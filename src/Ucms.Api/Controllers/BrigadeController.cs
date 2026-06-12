namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Brigades.Commands;
using Ucms.Application.Features.Brigades.Queries;

/// <summary>
/// Brigadalarni boshqarish.
/// Управление бригадами.
/// </summary>
[ApiController]
[Route("api/brigades")]
[Tags("Brigade")]
[Authorize]
public class BrigadeController(
    GetBrigades.Handler    getAll,
    GetBrigadeById.Handler getById,
    CreateBrigade.Handler  create,
    UpdateBrigade.Handler  update,
    DeleteBrigade.Handler  delete) : ControllerBase
{
    public record CreateBrigadeRequest(string Name, string? ForemanName, string? Phone, string? Notes);

    public record UpdateBrigadeRequest(string Name, string? ForemanName, string? Phone, bool IsActive, string? Notes);

    /// <summary>
    /// Brigadalar ro'yxati.
    /// Список бригад.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive,
        [FromQuery] string? status,
        CancellationToken ct = default)
    {
        return Ok(await getAll.HandleAsync(new(isActive, status), ct));
    }

    /// <summary>
    /// ID bo'yicha brigadani olish.
    /// Получить бригаду по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var (data, forbidden) = await getById.HandleAsync(new(id), ct);
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    /// <summary>
    /// Yangi brigada yaratish. Admin yoki Manager uchun.
    /// Создать новую бригаду. Для Admin или Manager.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateBrigadeRequest req, CancellationToken ct)
    {
        var result = await create.HandleAsync(new(req.Name, req.ForemanName, req.Phone, req.Notes), ct);
        if (result is null) return BadRequest(new { message = "Foydalanuvchiga tashkilot biriktirilmagan. / Пользователю не привязана организация." });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Brigadani yangilash. Admin yoki Manager uchun.
    /// Обновить бригаду. Для Admin или Manager.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrigadeRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await update.HandleAsync(new(id, req.Name, req.ForemanName, req.Phone, req.IsActive, req.Notes), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    /// <summary>
    /// Brigadani o'chirish. Admin yoki Manager uchun.
    /// Удалить бригаду. Для Admin или Manager.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var (notFound, forbidden) = await delete.HandleAsync(new(id), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }
}
