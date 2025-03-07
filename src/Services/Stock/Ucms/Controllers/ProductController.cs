namespace Ucms.Stock.Api.Controllers;

using Devhub.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Common.Models;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Consumers.Product;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Contracts.Requests.Products;
using static Ucms.Core.Constants.Permissions.AdminV1;

[Route("api/products")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IMediatorWrapper _mediator;

    public ProductController(IMediatorWrapper mediatorWrapper)
    {
        _mediator = mediatorWrapper;
    }

    /// <summary>
    /// Получить коллекцию всех продуктов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<ProductModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest request)
    {
        var response = await _mediator.Send(new GetProductsMessage(request.Query, request.Type, request));
        return Ok(response);
    }

    /// <summary>
    /// Получить коллекцию всех продуктов по организацию
    /// </summary>
    [HttpGet("organization")]
    [ProducesResponseType(typeof(PagedList<ProductModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizationProducts([FromQuery] GetOrganizationProductsRequest request)
    {
        var response = await _mediator.Send(new GetOrganizationProductsMessage(request.OrganizationId, request.StockId, request));
        return Ok(response);
    }

    /// <summary>
    /// Получить коллекцию продуктов фильтруется
    /// </summary>
    [HttpPost("table-list")]
    [ProducesResponseType(typeof(TableDataResult<ProductModel[]>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchProducts([FromBody] FilteringRequest filter)
    {
        var response = await _mediator.Send(new GetFilteredProductsMessage(filter));
        return Ok(response);
    }

    /// <summary>
    /// Получить продукт по id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var response = await _mediator.Send(new GetProductMessage(id));
        return Ok(response);
    }

    /// <summary>
    /// Получить продукт по коду
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ProductModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCode(string code)
    {
        var response = await _mediator.Send(new FindCodeProductMessage(code));
        return Ok(response);
    }

    /// <summary>
    /// Найдите продукт по названию
    /// </summary>
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(ProductModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductByName(string name)
    {
        var response = await _mediator.Send(new FindNameProductMessage(name));
        return Ok(response);
    }

    /// <summary>
    /// Создать продукт
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    // [HasPermissions(AddDirectories)]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        var response = await _mediator.Send(new CreateProductMessage(
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.Code,
            request.InternationalCode,
            request.InternationalName,
            request.AlternativeName,
            request.Type));
        return Ok(response);
    }

    /// <summary>
    /// Обновить продукт
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    // [HasPermissions(EditDirectories)]
    public async Task<IActionResult> UpdateProduct(UpdateProductRequest request)
    {
        var response = await _mediator.Send(new UpdateProductMessage(
            request.Id,
            request.Name,
            request.NameRu,
            request.NameEn,
            request.NameKa,
            request.Code,
            request.InternationalCode,
            request.InternationalName,
            request.AlternativeName,
            request.Type));
        return Ok(response);
    }

    /// <summary>
    /// Удалить продукт по id
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(DeleteDirectories)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var response = await _mediator.Send(new DeleteProductMessage(id));
        return Ok(response);
    }

    /// <summary>
    /// Удалить коллекцию товаров
    /// </summary>
    [HttpPost("delete-range")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    // [HasPermissions(DeleteDirectories)]
    public async Task<IActionResult> DeleteProducts(Guid[] guids)
    {
        var response = await _mediator.Send(new DeleteProductsMessage(guids));
        return Ok(response);
    }
}
