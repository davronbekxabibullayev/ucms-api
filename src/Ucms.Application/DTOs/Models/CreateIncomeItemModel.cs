namespace Ucms.Application.DTOs.Models;

public record CreateIncomeItemModel(
    Guid SkuId,
    Guid MeasurementUnitId,
    decimal Amount
);
