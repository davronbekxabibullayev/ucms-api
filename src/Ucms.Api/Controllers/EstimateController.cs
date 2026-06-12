namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Estimates;

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
        Guid SectionId, string Name, string Unit, decimal Volume,
        decimal ClientUnitPrice, decimal BrigadeUnitPrice, int Order);

    public record UpdateItemRequest(
        string Name, string Unit, decimal Volume,
        decimal ClientUnitPrice, decimal BrigadeUnitPrice, int Order);

    // ── Sections ───────────────────────────────────────────────────────────────

    [HttpGet("sections")]
    [Authorize(Roles = "Admin,Manager,Brigadir,Accountant")]
    public async Task<IActionResult> GetSections(Guid projectId, CancellationToken ct)
    {
        var (data, forbidden) = await getSections.HandleAsync(new(projectId), ct);
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPost("sections")]
    public async Task<IActionResult> CreateSection(
        Guid projectId, [FromBody] CreateSectionRequest req, CancellationToken ct)
    {
        var (data, forbidden) = await createSection.HandleAsync(new(projectId, req.Name, req.Order), ct);
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPut("sections/{sectionId:guid}")]
    public async Task<IActionResult> UpdateSection(
        Guid projectId, Guid sectionId, [FromBody] UpdateSectionRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await updateSection.HandleAsync(
            new(projectId, sectionId, req.Name, req.Order), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    [HttpDelete("sections/{sectionId:guid}")]
    public async Task<IActionResult> DeleteSection(
        Guid projectId, Guid sectionId, CancellationToken ct)
    {
        var (notFound, forbidden) = await deleteSection.HandleAsync(new(projectId, sectionId), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    // ── Items ──────────────────────────────────────────────────────────────────

    [HttpGet("sections/{sectionId:guid}/items")]
    [Authorize(Roles = "Admin,Manager,Brigadir,Accountant")]
    public async Task<IActionResult> GetItems(
        Guid projectId, Guid sectionId, CancellationToken ct)
    {
        var (data, forbidden) = await getItems.HandleAsync(new(projectId, sectionId), ct);
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem(
        Guid projectId, [FromBody] CreateItemRequest req, CancellationToken ct)
    {
        var (data, forbidden, error) = await createItem.HandleAsync(
            new(projectId, req.SectionId, req.Name, req.Unit, req.Volume,
                req.ClientUnitPrice, req.BrigadeUnitPrice, req.Order), ct);
        if (forbidden)         return Forbid();
        if (error is not null) return BadRequest(new { message = error });
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid projectId, Guid itemId, [FromBody] UpdateItemRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await updateItem.HandleAsync(
            new(projectId, itemId, req.Name, req.Unit, req.Volume,
                req.ClientUnitPrice, req.BrigadeUnitPrice, req.Order), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> DeleteItem(
        Guid projectId, Guid itemId, CancellationToken ct)
    {
        var (notFound, forbidden) = await deleteItem.HandleAsync(new(projectId, itemId), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }
}
