namespace Ucms.Application.DTOs.Requests.StockSkus;

using QueryForge.Abstractions;

public class GetCaseSkusRequest
{
    public PagedRequest Filter { get; set; } = new();
    public Guid OrganizationId { get; set; }
    public Guid StockId { get; set; }
    public string? Seria { get; set; }
}
