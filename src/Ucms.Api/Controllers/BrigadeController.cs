namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Brigades;

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
    public record CreateBrigadeRequest(string Name, string? ForemanName, string? Phone);

    public record UpdateBrigadeRequest(string Name, string? ForemanName, string? Phone, bool IsActive);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive, CancellationToken ct)
    {
        return Ok(await getAll.HandleAsync(new(isActive), ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var (data, forbidden) = await getById.HandleAsync(new(id), ct);
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateBrigadeRequest req, CancellationToken ct)
    {
        var result = await create.HandleAsync(new(req.Name, req.ForemanName, req.Phone), ct);
        if (result is null) return BadRequest(new { message = "Foydalanuvchiga tashkilot biriktirilmagan" });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrigadeRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await update.HandleAsync(
            new(id, req.Name, req.ForemanName, req.Phone, req.IsActive), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var (notFound, forbidden) = await delete.HandleAsync(new(id), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }
}
