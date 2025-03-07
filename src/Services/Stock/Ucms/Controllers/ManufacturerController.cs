namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Models;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.Manufacturer;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.Manufacturers;
using static Ucms.Core.Constants.Permissions.AdminV1;

[Route("api/manufacturers")]
[ApiController]
[Authorize]
public class ManufacturerController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;

    public ManufacturerController(IMediatorWrapper mediatorWrapper)
    {
        _mediator = mediatorWrapper;
    }

    /// <summary>
    /// Получить коллекцию производителей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<ManufacturerModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManufacturers([FromQuery] GetManufacturersRequest request)
    {
        var response = await _mediator.Send(new GetManufacturersMessage(request.Query, request));

        return Ok(response);
    }

    /// <summary>
    /// Получить коллекцию производителей.
    /// </summary>
    [HttpGet("stock-sku")]
    [ProducesResponseType(typeof(PagedList<ManufacturerModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockSkuManufacturers([FromQuery] GetStockSkuManufacturersRequest request)
    {
        var response = await _mediator.Send(new GetStockSkuManufacturersMessage(
            request.Query,
            request.OrganizationId,
            request.StockId,
            request.ProductId,
            request));

        return Ok(response);
    }

    /// <summary>
    /// Получить отфильтрованную коллекцию производителей.
    /// </summary>
    [HttpPost("table-list")]
    [ProducesResponseType(typeof(TableDataResult<ManufacturerModel[]>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchManufacturers([FromBody] FilteringRequest filter)
    {
        var response = await _mediator.Send(new GetFilteredManufacturersMessage(filter));
        return Ok(response);
    }

    /// <summary>
    /// Получить производитель по идентификатор
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ManufacturerModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManufacturer(Guid id)
    {
        var response = await _mediator.Send(new GetManufacturerMessage(id));
        return Ok(response);
    }

    /// <summary>
    /// Получить производитель по коду
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ManufacturerModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManufacturerByCode(string code)
    {
        var response = await _mediator.Send(new FindCodeManufacturerMessage(code));
        return Ok(response);
    }

    /// <summary>
    /// Получить производитель по имени
    /// </summary>
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(ManufacturerModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManufacturerByName(string name)
    {
        var response = await _mediator.Send(new FindNameManufacturerMessage(name));
        return Ok(response);
    }

    /// <summary>
    /// Создать производителя
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    // [HasPermissions(AddDirectories)]
    public async Task<IActionResult> CreateManufacturer(CreateManufacturerMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Обновить производителя
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    // [HasPermissions(EditDirectories)]
    public async Task<IActionResult> UpdateManufacturer(UpdateManufacturerMessage command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Удалить производителя
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    // [HasPermissions(DeleteDirectories)]
    public async Task<IActionResult> DeleteManufacturer(Guid id)
    {
        var response = await _mediator.Send(new DeleteManufacturerMessage(id));
        return Ok(response);
    }

    /// <summary>
    /// Удалить коллекцию производителей
    /// </summary>
    [HttpPost("delete-range")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    // [HasPermissions(DeleteDirectories)]
    public async Task<IActionResult> DeleteManufacturers(Guid[] guids)
    {
        var response = await _mediator.Send(new DeleteManufacturersMessage(guids));
        return Ok(response);
    }
}
