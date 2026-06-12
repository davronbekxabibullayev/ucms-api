namespace Ucms.Application.Features.Incomes;

using Ucms.Application.Features.MeasurementUnits;
using Ucms.Application.Features.Skus;

public record IncomeItemModel(
    Guid Id,
    Guid SkuId,
    decimal Amount,
    Guid? MeasurementUnitId,
    SkuModel? Sku,
    MeasurementUnitModel? MeasurementUnit
);
