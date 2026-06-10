namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.MeasurementUnit;
using Ucms.Stock.Api.Application.Consumers.OrganizationMeasurementUnit;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.OrganizationMeasurementUnits;
using static Ucms.Core.Constants.Permissions.AdminV1;

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
    [ProducesResponseType(typeof(PagedList<OrganizationMeasurementUnitModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizationMeasurementUnitTable([FromBody] FilteringRequest filter)
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
