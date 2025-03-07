namespace Ucms.Stock.Contracts.Models;

using Ucms.Stock.Domain.Models.Enums;

public record StockModel(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string Code,
    StorageCondition StorageCondition,
    StockType StockType,
    StockCategory StockCategory,
    Guid OrganizationId,
    Guid? ParentId,
    Guid[] EmployeeIds
);
