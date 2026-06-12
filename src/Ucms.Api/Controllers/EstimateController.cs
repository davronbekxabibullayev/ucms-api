namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Estimates.Commands;
using Ucms.Application.Features.Estimates.Queries;

/// <summary>
/// Loyiha smeta bo'limlari va pozitsiyalarini boshqarish.
/// Управление разделами и позициями сметы проекта.
/// </summary>
[ApiController]
[Route("api/projects/{projectId:guid}/estimate")]
[Tags("Estimate")]
[Authorize(Roles = "Admin,Manager")]
public class EstimateController(
    GetSections.Handler    getSections,
    CreateSection.Handler  createSection,
    UpdateSection.Handler  updateSection,
    DeleteSection.Handler  deleteSection,
    GetItems.Handler       getItems,
    CreateItem.Handler     createItem,
    UpdateItem.Handler     updateItem,
    DeleteItem.Handler     deleteItem) : ControllerBase
{
    public record CreateSectionRequest(string Name, int Order);
    public record UpdateSectionRequest(string Name, int Order);

    public record CreateItemRequest(
        Guid SectionId, string Name, Guid MeasurementUnitId, decimal Volume,
        decimal ClientUnitPrice, decimal BrigadeUnitPrice, int Order);

    public record UpdateItemRequest(
        string Name, Guid MeasurementUnitId, decimal Volume,
        decimal ClientUnitPrice, decimal BrigadeUnitPrice, int Order);

    // ── Sections ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Loyiha smetasi bo'limlari ro'yxati.
    /// Список разделов сметы проекта.
    /// </summary>
    [HttpGet("sections")]
    [Authorize(Roles = "Admin,Manager,Brigadir,Accountant")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSections(Guid projectId, CancellationToken ct)
    {
        var (data, forbidden) = await getSections.HandleAsync(new(projectId), ct);
        if (forbidden) return Forbid();
        // Handler mavjud loyiha uchun bo'sh list qaytarishi kerak, null emas
        return data is null ? NotFound() : Ok(data);
    }

    /// <summary>
    /// Yangi smeta bo'limi yaratish.
    /// Создать новый раздел сметы.
    /// </summary>
    [HttpPost("sections")]
    [ProducesResponseType(201)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateSection(
        Guid projectId, [FromBody] CreateSectionRequest req, CancellationToken ct)
    {
        var (data, forbidden) = await createSection.HandleAsync(new(projectId, req.Name, req.Order), ct);
        if (forbidden) return Forbid();
        if (data is null) return NotFound();
        return StatusCode(201, data);
    }

    /// <summary>
    /// Smeta bo'limini yangilash.
    /// Обновить раздел сметы.
    /// </summary>
    [HttpPut("sections/{sectionId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateSection(
        Guid projectId, Guid sectionId, [FromBody] UpdateSectionRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await updateSection.HandleAsync(
            new(projectId, sectionId, req.Name, req.Order), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    /// <summary>
    /// Smeta bo'limini o'chirish.
    /// Удалить раздел сметы.
    /// </summary>
    [HttpDelete("sections/{sectionId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteSection(
        Guid projectId, Guid sectionId, CancellationToken ct)
    {
        var (notFound, forbidden) = await deleteSection.HandleAsync(new(projectId, sectionId), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    // ── Items ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Bo'lim ichidagi smeta pozitsiyalari ro'yxati.
    /// Список позиций сметы внутри раздела.
    /// </summary>
    [HttpGet("sections/{sectionId:guid}/items")]
    [Authorize(Roles = "Admin,Manager,Brigadir,Accountant")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetItems(
        Guid projectId, Guid sectionId, CancellationToken ct)
    {
        var (data, forbidden) = await getItems.HandleAsync(new(projectId, sectionId), ct);
        if (forbidden) return Forbid();
        // Handler mavjud bo'lim uchun bo'sh list qaytarishi kerak, null emas
        return data is null ? NotFound() : Ok(data);
    }

    /// <summary>
    /// Yangi smeta pozitsiyasi yaratish.
    /// Создать новую позицию сметы.
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateItem(
        Guid projectId, [FromBody] CreateItemRequest req, CancellationToken ct)
    {
        var (data, forbidden, error) = await createItem.HandleAsync(
            new(projectId, req.SectionId, req.Name, req.MeasurementUnitId, req.Volume,
                req.ClientUnitPrice, req.BrigadeUnitPrice, req.Order), ct);
        if (forbidden)         return Forbid();
        if (error is not null) return BadRequest(new { message = error });
        if (data is null)      return NotFound();
        return StatusCode(201, data);
    }

    /// <summary>
    /// Smeta pozitsiyasini yangilash.
    /// Обновить позицию сметы.
    /// </summary>
    [HttpPut("items/{itemId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateItem(
        Guid projectId, Guid itemId, [FromBody] UpdateItemRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await updateItem.HandleAsync(
            new(projectId, itemId, req.Name, req.MeasurementUnitId, req.Volume,
                req.ClientUnitPrice, req.BrigadeUnitPrice, req.Order), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    /// <summary>
    /// Smeta pozitsiyasini o'chirish.
    /// Удалить позицию сметы.
    /// </summary>
    [HttpDelete("items/{itemId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteItem(
        Guid projectId, Guid itemId, CancellationToken ct)
    {
        var (notFound, forbidden) = await deleteItem.HandleAsync(new(projectId, itemId), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }
}
