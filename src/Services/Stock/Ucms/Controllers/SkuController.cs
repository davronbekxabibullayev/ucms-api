namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.Sku;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.Skus;
using static Ucms.Core.Constants.Permissions;

[Route("api/sku")]
[ApiController]
[Authorize]
public class SkuController : ControllerBase
{
    private readonly IMediatorWrapper _mediatorWrapper;

    public SkuController(IMediatorWrapper mediatorWrapper)
    {
        _mediatorWrapper = mediatorWrapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SkuModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSku(Guid id)
    {
        var response = await _mediatorWrapper.Send(new GetSkuMessage(id));
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(SkuModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkus()
    {
        var response = await _mediatorWrapper.Send(new GetSkusMessage());
        return Ok(response);
    }

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedList<SkuModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchSkus([FromBody] GetSkusRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetFilteredSkusMessage(request, request.Query, request.Seria));
        return Ok(response);
    }

    [HttpGet("serial/{serial}")]
    [ProducesResponseType(typeof(SkuModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkuByCode(string serial)
    {
        var response = await _mediatorWrapper.Send(new FindSkuMessage(serial));
        return Ok(response);
    }

    [HttpGet("search/{query}")]
    [ProducesResponseType(typeof(SkuModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchSkuByQuery(string query)
    {
        var result = await _mediatorWrapper.Send(new FindSkusMessage(query));
        return Ok(result);
    }

    [HttpGet("product-stock")]
    [ProducesResponseType(typeof(SkuModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductStockSkus([FromQuery] GetProductStockSkusRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetProductStockSkusMessage(
            request,
            request.ProductId,
            request.StockId,
            request.Types,
            request.Query));
        return Ok(response);
    }

    [HttpGet("product/{id:guid}")]
    [ProducesResponseType(typeof(SkuModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductSkus(Guid id)
    {
        var response = await _mediatorWrapper.Send(new GetProductSkusMessage(id));
        return Ok(response);
    }

    [HttpGet("check-for-used/{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckForUsed(Guid id)
    {
        var response = await _mediatorWrapper.Send(new CheckSkuForUsedMessage(id));
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions(Warehouse.AccessAddInventoryUnit)]
    public async Task<IActionResult> CreateSku(CreateSkuRequest request)
    {
        var response = await _mediatorWrapper.Send(new CreateSkuMessage(
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.SerialNumber,
            request.ProductId,
            request.ManufacturerId,
            request.MeasurementUnitId,
            request.SupplierId,
            request.Price,
            request.Amount,
            request.ExpirationDate,
            request.Status
            ));
        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(Warehouse.AccessEditInventoryUnit)]
    public async Task<IActionResult> UpdateSku(UpdateSkuRequest request)
    {
        var response = await _mediatorWrapper.Send(new UpdateSkuMessage(
            request.Id,
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.SerialNumber,
            request.ProductId,
            request.ManufacturerId,
            request.MeasurementUnitId,
            request.SupplierId,
            request.Price,
            request.Amount,
            request.ExpirationDate,
            request.Status));
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(Warehouse.AccessDeleteInventoryUnit)]
    public async Task<IActionResult> DeleteSku(Guid id)
    {
        var response = await _mediatorWrapper.Send(new DeleteSkuMessage(id));
        return Ok(response);
    }

    [HttpPost("delete-range")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(Warehouse.AccessDeleteInventoryUnit)]
    public async Task<IActionResult> DeleteSkus(Guid[] ids)
    {
        var response = await _mediatorWrapper.Send(new DeleteSkusMessage(ids));
        return Ok(response);
    }
}
