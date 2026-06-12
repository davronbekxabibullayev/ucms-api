namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueryForge.Models;
using Ucms.Application.Abstractions.Storage;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Features.Outcomes;
using Ucms.Domain.Enums;

[Route("api/outcome")]
[ApiController]
[Authorize]
public class OutcomeController(
    GetOutcomes.Handler getOutcomes,
    GetOutcomeById.Handler getById,
    GetOutcomeByExecutionId.Handler getByExecutionId,
    FindOutcome.Handler findOutcome,
    FindOutcomes.Handler findOutcomes,
    GetFilteredOutcomes.Handler getFiltered,
    GetOutcomeStats.Handler getStats,
    CreateOutcome.Handler create,
    UpdateOutcome.Handler update,
    UpdateOutcomeStatus.Handler updateStatus,
    DeleteOutcome.Handler delete,
    UploadOutcomeFile.Handler uploadFile,
    IFileStorageClient storageClient) : ControllerBase
{
    public record GetOutcomesRequest(PagedRequest Filter, Guid? StockId, string? Query, DateTime? From, DateTime? To);
    public record GetOutcomeStatsRequest(Guid OrganizationId, DateTime From, DateTime To, DateTime PreviousFrom, DateTime PreviousTo);
    public record UpdateOutcomeStatusRequest(Guid Id, OutcomeStatus Status);
    public record CreateOutcomeRequest(string Name, string? Note, OutcomeType OutcomeType, OutcomeStatus OutcomeStatus,
        PaymentType PaymentType, DateTimeOffset OutcomeDate, Guid StockId, Guid? IncomeStockId,
        Guid? ExecutionId, IEnumerable<CreateOutcomeItemModel> OutcomeItems);
    public record UpdateOutcomeRequest(Guid Id, string Name, string? Note, OutcomeType OutcomeType, OutcomeStatus OutcomeStatus,
        PaymentType PaymentType, DateTimeOffset OutcomeDate, Guid StockId, Guid? IncomeStockId,
        Guid? ExecutionId, IEnumerable<CreateOutcomeItemModel> OutcomeItems);

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OutcomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcome(Guid id, CancellationToken ct)
    {
        var result = await getById.HandleAsync(new(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("execution/{executionId}")]
    [ProducesResponseType(typeof(OutcomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExecutionOutcome(Guid executionId, CancellationToken ct)
    {
        var result = await getByExecutionId.HandleAsync(new(executionId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OutcomeModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcomes(CancellationToken ct)
        => Ok(await getOutcomes.HandleAsync(new(), ct));

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedResult<OutcomeModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilteredOutcomes([FromBody] GetOutcomesRequest request, CancellationToken ct)
        => Ok(await getFiltered.HandleAsync(new(request.Filter, request.StockId, request.Query, request.From, request.To), ct));

    [HttpGet("name")]
    [ProducesResponseType(typeof(OutcomeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcomeByName([FromQuery] string name, CancellationToken ct)
        => Ok(await findOutcome.HandleAsync(new(name), ct));

    [HttpGet("search/{query}")]
    [ProducesResponseType(typeof(OutcomeModel[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchOutcomes(string query, CancellationToken ct)
        => Ok(await findOutcomes.HandleAsync(new(query), ct));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOutcome([FromBody] CreateOutcomeRequest request, CancellationToken ct)
    {
        var id = await create.HandleAsync(new(request.Name, request.Note, request.OutcomeType, request.OutcomeStatus,
            request.PaymentType, request.OutcomeDate, request.StockId, request.IncomeStockId, request.ExecutionId,
            request.OutcomeItems), ct);
        return Ok(id);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOutcome([FromBody] UpdateOutcomeRequest request, CancellationToken ct)
    {
        var found = await update.HandleAsync(new(request.Id, request.Name, request.Note, request.OutcomeType,
            request.OutcomeStatus, request.PaymentType, request.OutcomeDate, request.StockId, request.IncomeStockId,
            request.ExecutionId, request.OutcomeItems), ct);
        return found ? NoContent() : NotFound();
    }

    [HttpPut("update-status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOutcomeStatus([FromBody] UpdateOutcomeStatusRequest request, CancellationToken ct)
    {
        var (notFound, error) = await updateStatus.HandleAsync(new(request.Id, request.Status), ct);
        if (notFound) return NotFound();
        if (error is not null) return Conflict(error);
        return NoContent();
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(OutcomeStatsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutcomeStats([FromQuery] GetOutcomeStatsRequest request, CancellationToken ct)
        => Ok(await getStats.HandleAsync(new(request.OrganizationId, request.From, request.To, request.PreviousFrom, request.PreviousTo), ct));

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteOutcome(Guid id, CancellationToken ct)
    {
        var found = await delete.HandleAsync(new(id), ct);
        return found ? NoContent() : NotFound();
    }

    [HttpPost("upload/{id}")]
    [RequestSizeLimit(10L * 1024L * 1024L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L)]
    [ProducesResponseType(typeof(FileEntryModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Upload(Guid id, IFormFile file, CancellationToken ct)
    {
        var (result, error) = await uploadFile.HandleAsync(new(id, file), ct);
        if (error is not null) return BadRequest(error);
        return Ok(result);
    }

    [HttpGet("download/{id}")]
    [ProducesResponseType(typeof(FileEntryModel), StatusCodes.Status200OK)]
    public IActionResult Download(Guid id, [FromQuery] string path) => Ok();
}
