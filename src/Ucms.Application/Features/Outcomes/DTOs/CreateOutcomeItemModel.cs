namespace Ucms.Application.Features.Outcomes;

public record CreateOutcomeItemModel(
    Guid SkuId,
    Guid MeasurementUnitId,
    decimal Amount,
    decimal ActualAmount
);
