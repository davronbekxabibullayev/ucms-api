namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueryForge.Models;
using Ucms.Application.Abstractions.Storage;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Features.Incomes;
using Ucms.Domain.Enums;

[Route("api/income")]
[ApiController]
[Authorize]
public class IncomeController(
    GetIncomes.Handler getAll,
    GetIncomeById.Handler getById,
    FindIncome.Handler findByName,
    FindIncomes.Handler findMany,
    GetFilteredIncomes.Handler getFiltered,
    CreateIncome.Handler create,
    UpdateIncome.Handler update,
    UpdateIncomeStatus.Handler updateStatus,
    DeleteIncome.Handler delete,
    UploadIncomeFile.Handler upload,
    IFileStorageClient storageClient) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<IncomeModel>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
        => Ok(await getAll.HandleAsync(new(), ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IncomeModel), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await getById.HandleAsync(new(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> FindByName(string name, CancellationToken ct = default)
        => Ok(await findByName.HandleAsync(new(name), ct));

    [HttpGet("search/{query}")]
    public async Task<IActionResult> Search(string query, CancellationToken ct = default)
        => Ok(await findMany.HandleAsync(new(query), ct));

    public record GetIncomesRequest(PagedRequest Filter, Guid? StockId, string? Query, DateTime? From, DateTime? To);

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedResult<IncomeModel>), 200)]
    public async Task<IActionResult> GetFiltered([FromBody] GetIncomesRequest req, CancellationToken ct = default)
        => Ok(await getFiltered.HandleAsync(new(req.Filter, req.StockId, req.Query, req.From, req.To), ct));

    public record CreateIncomeRequest(string Name, string? Note, IncomeType IncomeType, IncomeStatus IncomeStatus,
        PaymentType PaymentType, DateTimeOffset IncomeDate, Guid StockId, IEnumerable<CreateIncomeItemModel> IncomeItems);

    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    public async Task<IActionResult> Create([FromBody] CreateIncomeRequest req, CancellationToken ct = default)
    {
        var id = await create.HandleAsync(
            new(req.Name, req.Note, req.IncomeType, req.IncomeStatus, req.PaymentType,
                req.IncomeDate, req.StockId, req.IncomeItems), ct);
        return Ok(id);
    }

    public record UpdateIncomeRequest(Guid Id, string Name, string? Note, IncomeType IncomeType, IncomeStatus IncomeStatus,
        PaymentType PaymentType, DateTimeOffset IncomeDate, Guid StockId, IEnumerable<CreateIncomeItemModel> IncomeItems);

    [HttpPut]
    [ProducesResponseType(typeof(Guid), 202)]
    public async Task<IActionResult> Update([FromBody] UpdateIncomeRequest req, CancellationToken ct = default)
    {
        var ok = await update.HandleAsync(
            new(req.Id, req.Name, req.Note, req.IncomeType, req.IncomeStatus, req.PaymentType,
                req.IncomeDate, req.StockId, req.IncomeItems), ct);
        return ok ? Ok(req.Id) : NotFound();
    }

    public record UpdateStatusRequest(Guid Id, IncomeStatus Status);

    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest req, CancellationToken ct = default)
    {
        var ok = await updateStatus.HandleAsync(new(req.Id, req.Status), ct);
        return ok ? Ok(req.Id) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var ok = await delete.HandleAsync(new(id), ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("upload/{id:guid}")]
    [RequestSizeLimit(10L * 1024L * 1024L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L)]
    [ProducesResponseType(typeof(FileEntryModel), 200)]
    public async Task<IActionResult> Upload(Guid id, IFormFile file, CancellationToken ct = default)
    {
        var (result, error) = await upload.HandleAsync(new(id, file), ct);
        return error is not null ? BadRequest(error) : Ok(result);
    }

    [HttpGet("download/{id:guid}")]
    public IActionResult Download(Guid id, [FromQuery] string path) => Ok();
}
