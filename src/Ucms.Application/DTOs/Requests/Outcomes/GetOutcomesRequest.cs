namespace Ucms.Application.DTOs.Requests.Outcomes;

using QueryForge.Abstractions;

public class GetOutcomesRequest
{
    public PagedRequest Filter { get; set; } = new();
    public Guid? StockId { get; set; }
    public string? Query { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
