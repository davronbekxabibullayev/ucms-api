namespace Ucms.Stock.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Ucms.Core.Services.Mediator;
using Microsoft.AspNetCore.Authorization;
using Ucms.Stock.Contracts.Requests.Stats;
using Ucms.Common.Contracts.Models.Dashboards;
using Ucms.Stock.Api.Application.Consumers.Stats;

[Route("api/stats")]
[ApiController]
[Authorize]
public class StockStatsController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;

    public StockStatsController(IMediatorWrapper mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stock-skues")]
    [ProducesResponseType(typeof(DashboardWidgetModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockSkuesStats([FromQuery] GetStocksStatisticsRequest request)
    {
        var response = await _mediator.Send(new GetAmbulanceStockStatsMessage(
            request.OrganizationId,
            request.RegionId,
            request.CityId));

        return Ok(response);
    }
}
