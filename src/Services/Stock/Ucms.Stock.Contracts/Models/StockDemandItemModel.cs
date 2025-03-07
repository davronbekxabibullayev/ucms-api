namespace Ucms.Stock.Contracts.Models;

public record StockDemandItemModel(
    Guid Id,
    Guid ProductId,
    Guid StockDemandId,
    Guid MeasurementUnitId,
    decimal Amount,
    string? Note,
    bool NotApproved,
    ProductModel? Product,
    MeasurementUnitModel? MeasurementUnit
);
