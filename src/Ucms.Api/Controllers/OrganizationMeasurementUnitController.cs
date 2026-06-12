namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Features.OrganizationMeasurementUnits;
using Ucms.Domain.Enums;

[Route("api/organization-measurement-unit")]
[ApiController]
[Authorize]
public class OrganizationMeasurementUnitController(
    GetOrganizationMeasurementUnits.Handler getAll,
    GetOrganizationMeasurementUnitById.Handler getById,
    GetFilteredOrganizationMeasurementUnits.Handler getFiltered,
    UpsertOrganizationMeasurementUnit.Handler upsert) : ControllerBase
{
    public record UpsertRequest(MeasurementUnitType Type, Guid MeasurementUnitId);

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrganizationMeasurementUnitModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await getById.HandleAsync(new(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrganizationMeasurementUnitModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await getAll.HandleAsync(new(), ct));

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedResult<OrganizationMeasurementUnitModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTable([FromBody] PagedRequest filter, CancellationToken ct)
        => Ok(await getFiltered.HandleAsync(new(filter), ct));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Upsert([FromBody] UpsertRequest request, CancellationToken ct)
        => Ok(await upsert.HandleAsync(new(request.Type, request.MeasurementUnitId), ct));
}
