namespace Ucms.Stock.Contracts.Requests.Skus;

using Ucms.Stock.Domain.Models.Enums;

public class GetProductStockSkusRequest : PagingRequest
{
    public Guid? ProductId { get; set; }
    public Guid? StockId { get; set; }
    public List<ProductType>? Types { get; set; }
    public string? Query { get; set; }
}
