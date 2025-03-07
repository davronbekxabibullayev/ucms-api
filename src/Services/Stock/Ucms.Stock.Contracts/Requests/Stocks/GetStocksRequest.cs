namespace Ucms.Stock.Contracts.Requests.Stocks;

using Ucms.Stock.Domain.Models.Enums;

public class GetStocksRequest
{
    public Guid OrganizationId { get; set; } = default!;
    public bool? Unattached { get; set; }
    public StockType? StockType { get; set; }
    public StockCategory? StockCategory { get; set; }
    public string? Query { get; set; }
    public bool? IncludeChild { get; set; }
}
