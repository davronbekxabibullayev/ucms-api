namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Organizations;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/organizations")]
[Tags("Organization")]
[Authorize]
public class OrganizationController(
    GetOrganizations.Handler    getAll,
    GetOrganizationById.Handler getById,
    CreateOrganization.Handler  create,
    UpdateOrganization.Handler  update,
    DeleteOrganization.Handler  delete) : ControllerBase
{
    public record CreateOrganizationRequest(
        string Name, string? TaxId, string? Address, string? Phone, string? Email,
        OrganizationType Type = OrganizationType.Tenant, bool IsTest = false);

    public record UpdateOrganizationRequest(
        string Name, string? TaxId, string? Address, string? Phone, string? Email,
        bool? IsTest = null);

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await getAll.HandleAsync(new(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var (data, forbidden) = await getById.HandleAsync(new(id), ct);
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest req, CancellationToken ct)
    {
        var result = await create.HandleAsync(
            new(req.Name, req.TaxId, req.Address, req.Phone, req.Email, req.Type, req.IsTest), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await update.HandleAsync(
            new(id, req.Name, req.TaxId, req.Address, req.Phone, req.Email, req.IsTest), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await delete.HandleAsync(new(id), ct) ? NoContent() : NotFound();
}
