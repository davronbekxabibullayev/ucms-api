namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Dashboard;

[ApiController]
[Route("api/dashboard")]
[Tags("Dashboard")]
[Authorize]
public class DashboardController(
    GetDashboard.Handler    getDashboard,
    GetProjectDetail.Handler getProjectDetail) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        return Ok(await getDashboard.HandleAsync(new(), ct));
    }

    [HttpGet("projects/{projectId:guid}")]
    public async Task<IActionResult> GetProjectDetail(Guid projectId, CancellationToken ct)
    {
        var (data, notFound, forbidden) = await getProjectDetail.HandleAsync(new(projectId), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return Ok(data);
    }
}
