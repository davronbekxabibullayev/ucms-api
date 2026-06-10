namespace Ucms.Application.DTOs.Requests.Skus;

using QueryForge.Abstractions;

using Ucms.Domain.Enums;

public class GetProductStockSkusRequest : PagedRequest
{
    public Guid? ProductId { get; set; }
    public Guid? StockId { get; set; }
    public List<ProductType>? Types { get; set; }
    public string? Query { get; set; }
}
