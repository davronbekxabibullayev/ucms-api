namespace Ucms.Application.DTOs.Requests.Products;

using QueryForge.Abstractions;

using Ucms.Domain.Enums;

public class GetProductsRequest : PagedRequest
{
    public string? Query { get; set; }
    public List<ProductType>? Type { get; set; }
}
