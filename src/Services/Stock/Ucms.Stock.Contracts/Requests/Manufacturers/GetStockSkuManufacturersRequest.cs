namespace Ucms.Stock.Contracts.Requests.Manufacturers;

public class GetStockSkuManufacturersRequest : PagingRequest
{
    public string? Query { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? StockId { get; set; }
    public Guid? ProductId { get; set; }
}
