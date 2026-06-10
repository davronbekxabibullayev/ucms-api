namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.Handlers.OrganizationMeasurementUnit;
using Ucms.Application.DTOs.Models;
using Ucms.Application.DTOs.Requests.OrganizationMeasurementUnits;
using Ucms.Application.Abstractions.Mediator;

[Route("api/organization-measurement-unit")]
[ApiController]
public class OrganizationMeasurementUnitController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;

    public OrganizationMeasurementUnitController(IMediatorWrapper mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrganizationMeasurementUnitModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizationMeasurementUnit(Guid id)
    {
        var response = await _mediator.Send(new GetOrganizationMeasurementUnitMessage(id));

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrganizationMeasurementUnitModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizationMeasurementUnits()
    {
        var response = await _mediator.Send(new GetOrganizationMeasurementUnitsMessage());

        return Ok(response);
    }

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedResult<OrganizationMeasurementUnitModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizationMeasurementUnitTable([FromBody] PagedRequest filter)
    {
        var response = await _mediator.Send(new GetFilteredOrganizationMeasurementUnitsMessage(filter));

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    //[HasPermissions(AddGlobalDirectories)]
    public async Task<IActionResult> Upsert(UpsertOrganizationMeasurementUnitRequest request)
    {
        var response = await _mediator.Send(new UpsertOrganizationMeasurementUnitMessage(request.Type, request.MeasurementUnitId));

        return Ok(response);
    }
}
