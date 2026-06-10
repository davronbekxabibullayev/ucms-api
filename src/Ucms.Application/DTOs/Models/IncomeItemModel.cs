namespace Ucms.Application.DTOs.Models;

public record IncomeItemModel(
    Guid Id,
    Guid SkuId,
    decimal Amount,
    Guid? MeasurementUnitId,
    SkuModel? Sku,
    MeasurementUnitModel? MeasurementUnit
);
