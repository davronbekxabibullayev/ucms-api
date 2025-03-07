namespace Ucms.Stock.Contracts.Requests.StockSkus;

public class GetStockInventoryRequest
{
    public FilteringRequest Filter { get; set; } = new();
    public Guid OrganizationId { get; set; } = default!;
    public Guid? StockId { get; set; }
    public Guid? ProductId { get; set; }
}
