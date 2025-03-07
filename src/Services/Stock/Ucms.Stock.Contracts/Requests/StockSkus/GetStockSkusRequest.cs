namespace Ucms.Stock.Contracts.Requests.StockSkus;

public class GetStockSkusRequest
{
    public FilteringRequest Filter { get; set; } = new();
    public Guid OrganizationId { get; set; } = default!;
    public Guid? StockId { get; set; }
    public Guid? MeasurementUnitId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? ManufacturerId { get; set; }
    public string? Seria { get; set; }
}
