namespace Ucms.Application.Features.StockSkus;

using Ucms.Application.Features.MeasurementUnits;
using Ucms.Application.Features.Skus;
using Ucms.Application.Features.Stocks;

public record StockSkuModel
{
    public decimal Amount { get; set; }

    public SkuModel? Sku { get; set; }
    public StockModel? Stock { get; set; }
    public MeasurementUnitModel? MeasurementUnit { get; set; }
}
