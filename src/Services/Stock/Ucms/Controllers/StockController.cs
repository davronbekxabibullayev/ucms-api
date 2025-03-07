namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.Stock;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.Stocks;
using static Ucms.Core.Constants.Permissions;

[Route("api/stock")]
[ApiController]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IMediatorWrapper _mediatorWrapper;

    public StockController(IMediatorWrapper mediatorWrapper)
    {
        _mediatorWrapper = mediatorWrapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StockModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStock(Guid id)
    {
        var response = await _mediatorWrapper.Send(new GetStockMessage(id));
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(StockModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStocks([FromQuery] GetStocksRequest request)
    {
        var message = new GetStocksMessage(
            request.OrganizationId,
            request.Unattached,
            request.StockType,
            request.StockCategory,
            request.Query,
            request.IncludeChild);
        var response = await _mediatorWrapper.Send(message);
        return Ok(response);
    }

    [HttpGet("cases")]
    [ProducesResponseType(typeof(StockModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCases()
    {
        var response = await _mediatorWrapper.Send(new GetAllCasesMessage());
        return Ok(response);
    }

    [HttpGet("central-stock/{organizationId}")]
    [ProducesResponseType(typeof(StockModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCentralStocks(Guid organizationId)
    {
        var response = await _mediatorWrapper.Send(new GetCentralStocksMessage(organizationId));
        return Ok(response);
    }

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedList<StockModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchStocks([FromBody] GetFilteredStocksRequest request)
    {
        var response = await _mediatorWrapper.Send(new GetFilteredStocksMessage(request));
        return Ok(response);
    }

    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(StockModel), StatusCodes.Status200OK)]
    [Obsolete(@"Use ""GET api/stock"" method.")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var response = await _mediatorWrapper.Send(new FindStockMessage(code));
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions(Warehouse.AccessAddWarehouse)]
    public async Task<IActionResult> CreateStock(CreateStockMessage command)
    {
        var response = await _mediatorWrapper.Send(command);
        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(Warehouse.AccessEditWarehouse)]
    public async Task<IActionResult> UpdateStock(UpdateStockMessage command)
    {
        var response = await _mediatorWrapper.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(Warehouse.AccessDeleteWarehouse)]
    public async Task<IActionResult> DeleteStock(Guid id)
    {
        var response = await _mediatorWrapper.Send(new DeleteStockMessage(id));
        return Ok(response);
    }
}
