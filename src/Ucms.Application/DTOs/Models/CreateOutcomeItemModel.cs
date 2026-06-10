namespace Ucms.Application.DTOs.Models;

public record CreateOutcomeItemModel(
    Guid SkuId,
    Guid MeasurementUnitId,
    decimal Amount,
    decimal ActualAmount
);
