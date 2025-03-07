namespace Ucms.Stock.Contracts.Models;

public record CreateOutcomeItemModel(
    Guid SkuId,
    Guid MeasurementUnitId,
    decimal Amount,
    decimal ActualAmount
);
