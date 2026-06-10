namespace Ucms.Application.DTOs.Requests.Manufacturers;

using QueryForge.Abstractions;

public class GetStockSkuManufacturersRequest : PagedRequest
{
    public string? Query { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? StockId { get; set; }
    public Guid? ProductId { get; set; }
}
