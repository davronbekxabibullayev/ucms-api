namespace Ucms.Application.Features.Incomes;

public record CreateIncomeItemModel(
    Guid SkuId,
    Guid MeasurementUnitId,
    decimal Amount
);
