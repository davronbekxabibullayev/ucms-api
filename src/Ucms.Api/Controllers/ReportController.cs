namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Handlers.Report;
using Ucms.Application.DTOs.Models;
using Ucms.Application.DTOs.Requests.Reports;
using Ucms.Application.Abstractions.Mediator;

[Route("api/reports")]
[ApiController]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;

    public ReportController(IMediatorWrapper mediatorWrapper)
    {
        _mediator = mediatorWrapper;
    }

    [HttpGet("product-balance")]
    [ProducesResponseType(typeof(ProductBalanceReportModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkuBalance([FromQuery] GetProductBalanceReportRequest request)
    {
        var response = await _mediator.Send(new GetProductBalanceReportMessage(
            request.From,
            request.To,
            request.OrganizationId));
        return Ok(response);
    }

    [HttpPost("export-product-balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportSkuBalance([FromBody] ProductBalanceReportModel data)
    {
        var result = await _mediator.Send(new GetProductBalanceExcelMessage(data));

        var fileName = $"Отчет остатков по периодам.xlsx";

        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
