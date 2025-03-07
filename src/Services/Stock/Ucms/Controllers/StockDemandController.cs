namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.StockDemand;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.StockDemands;
using static Ucms.Core.Constants.Permissions;

[Route("api/stock-demand")]
[ApiController]
[Authorize]
public class StockDemandController : ControllerBase
{
    private readonly IMediatorWrapper _mediatorWrapper;

    public StockDemandController(IMediatorWrapper mediatorWrapper)
    {
        _mediatorWrapper = mediatorWrapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StockDemandModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockDemand(Guid id)
    {
        var response = await _mediatorWrapper.Send(new GetStockDemandMessage(id));
        return Ok(response);
    }

    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(StockDemandModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockDemandByName(string name)
    {
        var response = await _mediatorWrapper.Send(new FindStockDemandMessage(name));
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(StockDemandModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockDemands()
    {
        var response = await _mediatorWrapper.Send(new GetStockDemandsMessage());
        return Ok(response);
    }

    [HttpPost("requested-demands")]
    [ProducesResponseType(typeof(PagedList<RequestedDemandModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestedDemands([FromBody] GetRequestedDemandsRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetRequestedDemandsMessage(
            request.Filter,
            request.From,
            request.To,
            request.Name));
        return Ok(response);
    }

    [HttpPost("received-demands")]
    [ProducesResponseType(typeof(PagedList<ReceivedDemandModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReceivedDemands([FromBody] GetReceivedDemandsRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetReceivedDemandsMessage(
            request.Filter,
            request.From,
            request.To,
            request.Name));
        return Ok(response);
    }

    [HttpGet("search/{query}")]
    [ProducesResponseType(typeof(StockDemandModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(string query)
    {
        var result = await _mediatorWrapper.Send(new FindStockDemandsMessage(query));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions(Warehouse.AccessGenerateNeedsRequest)]
    public async Task<IActionResult> CreateStockDemand(CreateStockDemandMessage command)
    {
        var response = await _mediatorWrapper.Send(command);
        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> UpdateStockDemand(UpdateStockDemandMessage command)
    {
        var response = await _mediatorWrapper.Send(command);
        return Ok(response);
    }

    [HttpPut("update-status")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(Warehouse.AccessApproveOrRejectReceivedNeeds)]
    public async Task<IActionResult> UpdateDemandStatus(UpdateStockDemandStatusMessage command)
    {
        var response = await _mediatorWrapper.Send(command);
        return Ok(response);
    }

    [HttpPut("update-broadcast-status")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> UpdateDemandBroadcastStatus(UpdateStockDemandBroadcastStatusMessage command)
    {
        var response = await _mediatorWrapper.Send(command);
        return Ok(response);
    }
}
