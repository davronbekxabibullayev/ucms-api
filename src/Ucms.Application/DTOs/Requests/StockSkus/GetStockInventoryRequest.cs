namespace Ucms.Application.DTOs.Requests.StockSkus;

using QueryForge.Abstractions;

public class GetStockInventoryRequest
{
    public PagedRequest Filter { get; set; } = new();
    public Guid OrganizationId { get; set; } = default!;
    public Guid? StockId { get; set; }
    public Guid? ProductId { get; set; }
}
