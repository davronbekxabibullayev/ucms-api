namespace Ucms.Stock.Contracts.Requests.Products;

using Ucms.Stock.Domain.Models.Enums;

public class GetProductsRequest : PagingRequest
{
    public string? Query { get; set; }
    public List<ProductType>? Type { get; set; }
}
