namespace Ucms.Stock.Contracts.Requests.StockDemands;

public class GetReceivedDemandsRequest
{
    public FilteringRequest Filter { get; set; } = new();
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Name { get; set; }
}
