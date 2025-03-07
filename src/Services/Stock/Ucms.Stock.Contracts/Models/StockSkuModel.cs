namespace Ucms.Stock.Contracts.Models;

public record StockSkuModel
{
    public decimal Amount { get; set; }

    public SkuModel? Sku { get; set; }
    public StockModel? Stock { get; set; }
    public MeasurementUnitModel? MeasurementUnit { get; set; }
}
