namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Projects;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/projects")]
[Tags("Project")]
[Authorize]
public class ProjectController(
    GetProjects.Handler    getAll,
    GetProjectById.Handler getById,
    CreateProject.Handler  create,
    UpdateProject.Handler  update,
    DeleteProject.Handler  delete) : ControllerBase
{
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

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ProjectStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        var (data, forbidden) = await getAll.HandleAsync(new(status, page, size), ct);
        if (forbidden) return Forbid();
        return Ok(data);
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
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest req, CancellationToken ct)
    {
        var result = await create.HandleAsync(
            new(req.Name, req.Address, req.Description, req.ContractNumber,
                req.ContractDate, req.StartDate, req.EndDate), ct);

        if (result is null) return BadRequest(new { message = "Foydalanuvchiga tashkilot biriktirilmagan" });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await update.HandleAsync(
            new(id, req.Name, req.Address, req.Description, req.ContractNumber,
                req.ContractDate, req.StartDate, req.EndDate, req.Status), ct);

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
