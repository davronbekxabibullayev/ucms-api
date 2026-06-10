namespace Ucms.Stock.Contracts.Models;

public record CreateIncomeItemModel(
    Guid SkuId,
    Guid MeasurementUnitId,
    decimal Amount
);
