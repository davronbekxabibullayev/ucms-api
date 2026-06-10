namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.MeasurementUnit;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.MeasurementUnits;
using static Ucms.Core.Constants.Permissions.AdminV1;

[Route("api/measurement-unit")]
[ApiController]
[Authorize]
public class MeasurementUnitController : ControllerBase
{
    private readonly IMediatorWrapper _mediatorWrapper;

    public MeasurementUnitController(IMediatorWrapper mediatorWrapper)
    {
        _mediatorWrapper = mediatorWrapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MeasurementUnitModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMeasurementUnit(Guid id)
    {
        var response = await _mediatorWrapper.Send(new GetMeasurementUnitMessage(id));
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(MeasurementUnitModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMeasurementUnits()
    {
        var response = await _mediatorWrapper.Send(new GetMeasurementUnitsMessage());
        return Ok(response);
    }

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedList<MeasurementUnitModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilteredMeasurementUnits([FromBody] GetMeasurementUnitsRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetFilteredMeasurementUnitsMessage(request, request.Query));
        return Ok(response);
    }

    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(MeasurementUnitModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMeasurementUnitByCode(string code)
    {
        var response = await _mediatorWrapper.Send(new FindMeasurementUnitMessage(code));
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions(AddGlobalDirectories)]
    public async Task<IActionResult> CreateMeasurementUnit(CreateMeasurementUnitRequest request)
    {
        var response = await _mediatorWrapper.Send(new CreateMeasurementUnitMessage(
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.Code,
            request.Multiplier,
            request.Type));

        return Ok(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(EditGlobalDirectories)]
    public async Task<IActionResult> UpdateMeasurementUnit(Guid id, UpdateMeasurementUnitRequest request)
    {
        var response = await _mediatorWrapper.Send(new UpdateMeasurementUnitMessage(
            id,
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.Code,
            request.Type,
            request.Multiplier));

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(DeleteGlobalDirectories)]
    public async Task<IActionResult> DeleteMeasurementUnit(Guid id)
    {
        var response = await _mediatorWrapper.Send(new DeleteMeasurementUnitMessage(id));
        return Ok(response);
    }

    [HttpPost("delete-range")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(DeleteGlobalDirectories)]
    public async Task<IActionResult> DeleteMeasurementUnits(Guid[] ids)
    {
        var response = await _mediatorWrapper.Send(new DeleteMeasurementUnitsMessage(ids));
        return Ok(response);
    }
}
