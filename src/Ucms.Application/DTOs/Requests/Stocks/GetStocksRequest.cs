namespace Ucms.Application.DTOs.Requests.Stocks;

using Ucms.Domain.Enums;

public class GetStocksRequest
{
    public Guid OrganizationId { get; set; } = default!;
    public bool? Unattached { get; set; }
    public StockType? StockType { get; set; }
    public StockCategory? StockCategory { get; set; }
    public string? Query { get; set; }
    public bool? IncludeChild { get; set; }
}
