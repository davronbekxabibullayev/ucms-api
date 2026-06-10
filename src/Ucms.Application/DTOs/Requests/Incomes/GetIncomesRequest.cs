namespace Ucms.Application.DTOs.Requests.Incomes;

using QueryForge.Abstractions;

public class GetIncomesRequest
{
    public PagedRequest Filter { get; set; } = new();
    public Guid? StockId { get; set; }
    public string? Query { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
