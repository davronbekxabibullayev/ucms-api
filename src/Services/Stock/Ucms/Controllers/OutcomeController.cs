namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.Outcome;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.Outcomes;
using Ucms.Storage.Client;
using Ucms.Storage.Contracts.Models;
using static Ucms.Core.Constants.Permissions;

[Route("api/outcome")]
[ApiController]
[Authorize]
public class OutcomeController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;
    private readonly IFileStorageClient _storageClient;

    public OutcomeController(IMediatorWrapper mediator, IFileStorageClient storageClient)
    {
        _mediator = mediator;
        _storageClient = storageClient;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OutcomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcome(Guid id)
    {
        var response = await _mediator.Send(new GetOutcomeMessage(id));
        return Ok(response);
    }

    [HttpGet("execution/{executionId}")]
    [ProducesResponseType(typeof(OutcomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExecutionOutcome(Guid executionId)
    {
        var response = await _mediator.Send(new GetExecutionOutcomeMessage(executionId));
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OutcomeModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcomes()
    {
        var response = await _mediator.Send(new GetOutcomesMessage());
        return Ok(response);
    }

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedList<OutcomeModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilteredOutcomes([FromBody] GetOutcomesRequest request)
    {
        var response = await _mediator.Send(new GetFilteredOutcomesMessage(
            request.Filter,
            request.StockId,
            request.Query,
            request.From,
            request.To));
        return Ok(response);
    }

    [HttpGet("name")]
    [ProducesResponseType(typeof(OutcomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcomeByName([FromQuery] string name)
    {
        var response = await _mediator.Send(new FindOutcomeMessage(name));
        return Ok(response);
    }

    [HttpGet("search/{query}")]
    [ProducesResponseType(typeof(OutcomeModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchOutcomeByQuery(string query)
    {
        var result = await _mediator.Send(new FindOutcomesMessage(query));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions(Warehouse.AccessGenerateExpense)]
    public async Task<IActionResult> CreateOutcome(CreateOutcomeMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> UpdateOutcome(UpdateOutcomeMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("update-status")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(Warehouse.AccessApproveOrRejectExpense)]
    public async Task<IActionResult> UpdateOutcomeStatus(UpdateOutcomeStatusMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(OutcomeStatsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcomeStats([FromQuery] GetOutcomeStatsRequest request)
    {
        var response = await _mediator.Send(new GetOutcomeStatsMessage(
            request.OrganizationId,
            request.From,
            request.To,
            request.PreviousFrom,
            request.PreviousTo));

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteOutcome(Guid id)
    {
        var response = await _mediator.Send(new DeleteOutcomeMessage(id));
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
        var response = await _mediator.Send(new UploadOutcomeFileMessage(id, file));

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
