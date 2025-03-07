namespace Ucms.Stock.Contracts.Requests.Stocks;

using Ucms.Stock.Domain.Models.Enums;

public record CreateStockRequest
{
    public string Name { get; init; } = default!;
    public string NameRu { get; init; } = default!;
    public string? NameEn { get; init; }
    public string? NameKa { get; init; }
    public string Code { get; init; } = default!;
    public StorageCondition StorageCondition { get; init; }
    public StockType StockType { get; init; }
    public StockCategory StockCategory { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid? ParentId { get; init; }
    public Guid[] EmployeeIds { get; set; } = [];
}
