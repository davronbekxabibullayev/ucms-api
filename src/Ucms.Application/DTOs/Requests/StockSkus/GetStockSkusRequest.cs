namespace Ucms.Application.DTOs.Requests.StockSkus;

using QueryForge.Abstractions;

public class GetStockSkusRequest
{
    public PagedRequest Filter { get; set; } = new();
    public Guid OrganizationId { get; set; } = default!;
    public Guid? StockId { get; set; }
    public Guid? MeasurementUnitId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? ManufacturerId { get; set; }
    public string? Seria { get; set; }
}
