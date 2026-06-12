namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Abstractions.Dashboards;
using Ucms.Application.Features.Stats;

[Route("api/stats")]
[ApiController]
[Authorize]
public class StockStatsController(GetAmbulanceStocksStats.Handler getStats) : ControllerBase
{
    [HttpGet("stock-skues")]
    [ProducesResponseType(typeof(DashboardWidgetModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockSkuesStats(
        [FromQuery] Guid? organizationId,
        [FromQuery] Guid? regionId,
        [FromQuery] Guid? cityId,
        CancellationToken ct)
        => Ok(await getStats.HandleAsync(new(organizationId, regionId, cityId), ct));
}
