namespace Ucms.Stock.Contracts.Requests.Outcomes;

public class GetOutcomesRequest
{
    public FilteringRequest Filter { get; set; } = new();
    public Guid? StockId { get; set; }
    public string? Query { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
