namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.MeasurementUnits;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/measurement-units")]
[Tags("Lookup")]
[Authorize]
public class MeasurementUnitController(
    GetMeasurementUnits.Handler     getAll,
    GetFilteredMeasurementUnits.Handler getFiltered,
    GetMeasurementUnitById.Handler  getById,
    FindMeasurementUnit.Handler     findByCode,
    CreateMeasurementUnit.Handler   create,
    UpdateMeasurementUnit.Handler   update,
    DeleteMeasurementUnit.Handler   deleteOne,
    DeleteMeasurementUnits.Handler  deleteBulk) : ControllerBase
{
    public record CreateUnitRequest(string Code, string Name, string NameRu,
        string? NameEn, string? NameKa, MeasurementUnitType Type, decimal Multiplier = 1);
    public record UpdateUnitRequest(string Name, string NameRu, string? NameEn, string? NameKa,
        string? Code, MeasurementUnitType Type, decimal Multiplier);
    public record DeleteBulkRequest(Guid[] Ids);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] MeasurementUnitType? type, CancellationToken ct)
        => Ok(await getAll.HandleAsync(new(type), ct));

    [HttpGet("filter")]
    public async Task<IActionResult> GetFiltered(
        [FromQuery] string? search, [FromQuery] MeasurementUnitType? type,
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
        => Ok(await getFiltered.HandleAsync(new(search, type, page, size), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await getById.HandleAsync(new(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("find/{code}")]
    public async Task<IActionResult> FindByCode(string code, CancellationToken ct)
    {
        var result = await findByCode.HandleAsync(new(code), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUnitRequest req, CancellationToken ct)
    {
        var (id, error) = await create.HandleAsync(
            new(req.Code, req.Name, req.NameRu, req.NameEn, req.NameKa, req.Type, req.Multiplier), ct);
        if (error is not null) return Conflict(error);
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitRequest req, CancellationToken ct)
    {
        var (notFound, error) = await update.HandleAsync(
            new(id, req.Name, req.NameRu, req.NameEn, req.NameKa, req.Code, req.Type, req.Multiplier), ct);
        if (notFound) return NotFound();
        if (error is not null) return Conflict(error);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var found = await deleteOne.HandleAsync(new(id), ct);
        return found ? NoContent() : NotFound();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBulk([FromBody] DeleteBulkRequest req, CancellationToken ct)
    {
        await deleteBulk.HandleAsync(new(req.Ids), ct);
        return NoContent();
    }
}
