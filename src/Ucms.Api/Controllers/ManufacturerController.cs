namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Features.Manufacturers;

[Route("api/manufacturers")]
[ApiController]
[Authorize]
public class ManufacturerController(
    GetManufacturers.Handler getAll,
    GetStockSkuManufacturers.Handler getStockSku,
    GetFilteredManufacturers.Handler getFiltered,
    GetManufacturerById.Handler getById,
    FindManufacturerByCode.Handler findByCode,
    FindManufacturerByName.Handler findByName,
    CreateManufacturer.Handler create,
    UpdateManufacturer.Handler update,
    DeleteManufacturer.Handler delete,
    DeleteManufacturers.Handler deleteRange) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ManufacturerModel>), 200)]
    public async Task<IActionResult> GetManufacturers([FromQuery] string? query,
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
        => Ok(await getAll.HandleAsync(new(query, page, size), ct));

    [HttpGet("stock-sku")]
    [ProducesResponseType(typeof(PagedResult<ManufacturerModel>), 200)]
    public async Task<IActionResult> GetStockSkuManufacturers([FromQuery] string? query,
        [FromQuery] Guid? organizationId, [FromQuery] Guid? stockId, [FromQuery] Guid? productId,
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
        => Ok(await getStockSku.HandleAsync(new(query, organizationId, stockId, productId, page, size), ct));

    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedResult<ManufacturerModel>), 200)]
    public async Task<IActionResult> SearchManufacturers([FromBody] PagedRequest filter, CancellationToken ct = default)
        => Ok(await getFiltered.HandleAsync(new(filter), ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ManufacturerModel), 200)]
    public async Task<IActionResult> GetManufacturer(Guid id, CancellationToken ct = default)
    {
        var result = await getById.HandleAsync(new(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> FindByCode(string code, CancellationToken ct = default)
        => Ok(await findByCode.HandleAsync(new(code), ct));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> FindByName(string name, CancellationToken ct = default)
        => Ok(await findByName.HandleAsync(new(name), ct));

    public record CreateManufacturerRequest(string Name, string NameRu, string? NameEn, string? NameKa, string? Code);

    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    public async Task<IActionResult> CreateManufacturer([FromBody] CreateManufacturerRequest req, CancellationToken ct = default)
    {
        var (id, error) = await create.HandleAsync(new(req.Name, req.NameRu, req.NameEn, req.NameKa, req.Code), ct);
        return error is not null ? Conflict(error) : Ok(id);
    }

    public record UpdateManufacturerRequest(Guid Id, string Name, string NameRu, string? NameEn, string? NameKa, string? Code);

    [HttpPut]
    [ProducesResponseType(typeof(Guid), 202)]
    public async Task<IActionResult> UpdateManufacturer([FromBody] UpdateManufacturerRequest req, CancellationToken ct = default)
    {
        var ok = await update.HandleAsync(new(req.Id, req.Name, req.NameRu, req.NameEn, req.NameKa, req.Code), ct);
        return ok ? Ok(req.Id) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteManufacturer(Guid id, CancellationToken ct = default)
    {
        var (notFound, error) = await delete.HandleAsync(new(id), ct);
        if (notFound) return NotFound();
        return error is not null ? Conflict(error) : NoContent();
    }

    [HttpPost("delete-range")]
    public async Task<IActionResult> DeleteManufacturers([FromBody] Guid[] ids, CancellationToken ct = default)
    {
        var (_, error) = await deleteRange.HandleAsync(new(ids), ct);
        return error is not null ? Conflict(error) : NoContent();
    }
}
