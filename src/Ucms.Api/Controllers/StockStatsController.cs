namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ucms.Application.DTOs.Requests.Stats;
using Ucms.Application.Handlers.Stats;
using Ucms.Application.Abstractions.Dashboards;
using Ucms.Application.Abstractions.Mediator;

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
