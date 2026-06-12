namespace Ucms.Application.Features.StockDemands;

using Ucms.Application.Features.MeasurementUnits;
using Ucms.Application.Features.Products;

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
