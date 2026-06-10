namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.Handlers.Supplier;
using Ucms.Application.DTOs.Models;
using Ucms.Application.DTOs.Requests.Suppliers;
using Ucms.Application.Abstractions.Mediator;

[Route("api/suppliers")]
[ApiController]
[Authorize]
public class SupplierController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;

    public SupplierController(IMediatorWrapper mediatorWrapper)
    {
        _mediator = mediatorWrapper;
    }

    /// <summary>
    /// Получить коллекцию всех поставщиков
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SupplierModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuppliers([FromQuery] GetSuppliersRequest request)
    {
        var response = await _mediator.Send(new GetSuppliersMessage(request.Query, request));

        return Ok(response);
    }

    /// <summary>
    /// Получить коллекцию фильтрованный поставщиков
    /// </summary>
    [HttpPost("table-list")]
    [ProducesResponseType(typeof(PagedResult<SupplierModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchSuppliers([FromBody] PagedRequest filter)
    {
        var response = await _mediator.Send(new GetFilteredSuppliersMessage(filter));

        return Ok(response);
    }

    /// <summary>
    /// Получить поставщик по идентификатор
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SupplierModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSupplier(Guid id)
    {
        var response = await _mediator.Send(new GetSupplierMessage(id));

        return Ok(response);
    }

    /// <summary>
    /// Получить поставщик по коду
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(SupplierModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuppliersByCode(string code)
    {
        var response = await _mediator.Send(new FindCodeSupplierMessage(code));

        return Ok(response);
    }

    /// <summary>
    /// Создать поставщик
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions(AddDirectories)]
    public async Task<IActionResult> CreateSupplier(CreateSupplierRequest request)
    {
        var response = await _mediator.Send(new CreateSupplierMessage(
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.Code));

        return Ok(response);
    }

    /// <summary>
    /// Обновить поставщик
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(EditDirectories)]
    public async Task<IActionResult> UpdateSupplier(UpdateSupplierRequest request)
    {
        var response = await _mediator.Send(new UpdateSupplierMessage(
            request.Id,
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.Code));

        return Ok(response);
    }

    /// <summary>
    /// Удалить поставщик по идентификатору
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(DeleteDirectories)]
    public async Task<IActionResult> DeleteSupplier(Guid id)
    {
        var response = await _mediator.Send(new DeleteSupplierMessage(id));

        return Ok(response);
    }

    /// <summary>
    /// Удалить коллекцию поставщиков
    /// </summary>
    [HttpPost("delete-range")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(DeleteDirectories)]
    public async Task<IActionResult> DeleteSuppliers(Guid[] guids)
    {
        var response = await _mediator.Send(new DeleteSuppliersMessage(guids));

        return Ok(response);
    }
}
