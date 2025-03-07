namespace Ucms.Stock.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.Sku;
using Ucms.Stock.Api.Application.Consumers.StockSku;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.StockSkus;

[Route("api/stock-sku")]
[ApiController]
[Authorize]
public class StockSkuController : ControllerBase
{
    private readonly IMediatorWrapper _mediatorWrapper;

    public StockSkuController(IMediatorWrapper mediatorWrapper)
    {
        _mediatorWrapper = mediatorWrapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PagedList<StockSkuModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockSkus([FromBody] GetStockSkusRequest request)
    {
        var message = new GetStockSkusMessage(
            request.OrganizationId,
            request.Filter,
            request.StockId,
            request.MeasurementUnitId,
            request.ProductId,
            request.ManufacturerId,
            request.Seria);
        var response = await _mediatorWrapper.Send(message);
        return Ok(response);
    }

    [HttpPost("case-sku")]
    [ProducesResponseType(typeof(PagedList<StockSkuModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCaseSkus([FromBody] GetCaseSkusRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetCaseSkusMessage(
            request.Filter,
            request.OrganizationId,
            request.StockId,
            request.Seria));
        return Ok(response);
    }

    [HttpPost("inventory")]
    [ProducesResponseType(typeof(PagedList<StockInventoryModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockInventory([FromBody] GetStockInventoryRequest request)
    {
        var message = new GetStockInventoryMessage(
            request.Filter,
            request.StockId,
            request.ProductId,
            request.OrganizationId);
        var response = await _mediatorWrapper.Send(message);
        return Ok(response);
    }

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedList<StockSkuModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchStockSkus([FromBody] FilteringRequest filter)
    {
        var response = await _mediatorWrapper.Send(new GetFilteredStockSkusMessage(filter));

        return Ok(response);
    }

    [HttpGet("check-amount")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckForAmount([FromQuery] CheckForSkuAmountRequest request)
    {
        var response = await _mediatorWrapper.Send(new CheckForSkuAmountMessage(
            request.SkuId,
            request.StockId,
            request.MeasurementUnitId,
            request.Amount));

        return Ok(response);
    }

    [HttpGet("product-balance")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductBalance([FromQuery] GetProductBalanceRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetProductBalanceMessage(
            request.StockId,
            request.ProductId,
            request.MeasurementUnitId));

        return Ok(response);
    }

    [HttpGet("stats/{organizationId:guid}")]
    [ProducesResponseType(typeof(StockSkuStatModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockSkuStats(Guid organizationId)
    {
        var response = await _mediatorWrapper.Send(new GetStockSkuStatsMessage(organizationId));

        return Ok(response);
    }
}
