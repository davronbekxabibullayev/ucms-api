namespace Ucms.Stock.Contracts.Requests.StockSkus;

public class GetCaseSkusRequest
{
    public FilteringRequest Filter { get; set; } = new();
    public Guid OrganizationId { get; set; }
    public Guid StockId { get; set; }
    public string? Seria { get; set; }
}
