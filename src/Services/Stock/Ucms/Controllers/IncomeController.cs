namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.Income;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.Incomes;
using Ucms.Storage.Client;
using Ucms.Storage.Contracts.Models;
using static Ucms.Core.Constants.Permissions;

[Route("api/income")]
[ApiController]
[Authorize]
public class IncomeController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;
    private readonly IFileStorageClient _storageClient;

    public IncomeController(IMediatorWrapper mediator, IFileStorageClient storageClient)
    {
        _mediator = mediator;
        _storageClient = storageClient;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IncomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIncome(Guid id)
    {
        var response = await _mediator.Send(new GetIncomeMessage(id));
        return Ok(response);
    }

    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(IncomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIncomeByName(string name)
    {
        var response = await _mediator.Send(new FindIncomeMessage(name));
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IncomeModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIncomes()
    {
        var response = await _mediator.Send(new GetIncomesMessage());
        return Ok(response);
    }

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedList<IncomeModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilteredIncomes([FromBody] GetIncomesRequest request)
    {
        var response = await _mediator.Send(new GetFilteredIncomesMessage(
            request.Filter,
            request.StockId,
            request.Query,
            request.From,
            request.To));
        return Ok(response);
    }

    [HttpGet("search/{query}")]
    [ProducesResponseType(typeof(IncomeModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchIncome(string query)
    {
        var result = await _mediator.Send(new FindIncomesMessage(query));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions([Warehouse.AccessGenerateIncome])]
    public async Task<IActionResult> CreateIncome(CreateIncomeMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> UpdateIncome(UpdateIncomeMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("update-status")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(Warehouse.AccessApproveOrRejectIncome)]
    public async Task<IActionResult> UpdateStatusOfIncome(UpdateIncomeStatusMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteIncome(Guid id)
    {
        var response = await _mediator.Send(new DeleteIncomeMessage(id));
        return Ok(response);
    }

    /// <summary>
    /// Uploads a fire object file
    /// </summary>
    [HttpPost("upload/{id}")]
    [RequestSizeLimit(10L * 1024L * 1024L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L)]
    [ProducesResponseType(typeof(FileEntryModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Upload(Guid id, IFormFile file)
    {
        var response = await _mediator.Send(new UploadIncomeFileMessage(id, file));

        return Ok(response);
    }

    /// <summary>
    /// Downloads a specific file
    /// </summary>
    [HttpGet("download/{id}")]
    [ProducesResponseType(typeof(FileEntryModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(Guid id, [FromQuery] string path)
    {
        try
        {
            var response = await _storageClient.DownloadAsync($"{path}/{id}.pdf");
            return File(response, "application/octet-stream");
        }
        catch
        {
            return BadRequest("File not found");
        }
    }
}
