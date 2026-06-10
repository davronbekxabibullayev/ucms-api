namespace Ucms.Application.DTOs.Requests.StockDemands;

using QueryForge.Abstractions;

public class GetRequestedDemandsRequest
{
    public PagedRequest Filter { get; set; } = new();
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Name { get; set; }
}
