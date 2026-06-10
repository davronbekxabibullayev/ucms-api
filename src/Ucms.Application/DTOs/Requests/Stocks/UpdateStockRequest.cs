namespace Ucms.Application.DTOs.Requests.Stocks;

using Ucms.Domain.Enums;

public record UpdateStockRequest
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
