namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.ClientActs;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/projects/{projectId:guid}/acts")]
[Tags("ClientAct")]
[Authorize]
public class ClientActController(
    GetClientActs.Handler       getAll,
    GetClientActById.Handler    getById,
    CreateClientAct.Handler     create,
    UpdateClientActStatus.Handler updateStatus,
    DeleteClientAct.Handler     delete) : ControllerBase
{
    public record ActItemDto(Guid EstimateItemId, decimal Volume, decimal UnitPrice);

    public record CreateActRequest(
        string ActNumber, DateTimeOffset ActDate,
        List<ActItemDto> Items, string? Note);

    public record UpdateActStatusRequest(ActStatus Status);

    [HttpGet]
    public async Task<IActionResult> GetAll(
        Guid projectId, [FromQuery] ActStatus? status, CancellationToken ct)
    {
        var (data, notFound, forbidden) = await getAll.HandleAsync(new(projectId, status), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid projectId, Guid id, CancellationToken ct)
    {
        var (data, notFound, forbidden) = await getById.HandleAsync(new(projectId, id), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> Create(
        Guid projectId, [FromBody] CreateActRequest req, CancellationToken ct)
    {
        var items = req.Items.Select(i =>
            new CreateClientAct.ActItemDto(i.EstimateItemId, i.Volume, i.UnitPrice)).ToList();

        var (data, notFound, forbidden) = await create.HandleAsync(
            new(projectId, req.ActNumber, req.ActDate, items, req.Note), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return CreatedAtAction(nameof(GetById), new { projectId, id = data!.Id }, data);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> UpdateStatus(
        Guid projectId, Guid id, [FromBody] UpdateActStatusRequest req, CancellationToken ct)
    {
        var (notFound, projectNotFound, forbidden) = await updateStatus.HandleAsync(
            new(projectId, id, req.Status), ct);
        if (projectNotFound) return NotFound();
        if (notFound)        return NotFound();
        if (forbidden)       return Forbid();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id, CancellationToken ct)
    {
        var (notFound, projectNotFound, forbidden, error) = await delete.HandleAsync(new(projectId, id), ct);
        if (projectNotFound)   return NotFound();
        if (notFound)          return NotFound();
        if (forbidden)         return Forbid();
        if (error is not null) return BadRequest(new { message = error });
        return NoContent();
    }
}
