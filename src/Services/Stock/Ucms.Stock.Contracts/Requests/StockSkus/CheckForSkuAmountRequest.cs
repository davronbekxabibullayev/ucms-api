namespace Ucms.Stock.Contracts.Requests.StockSkus;

public class CheckForSkuAmountRequest
{
    public Guid SkuId { get; set; } = default!;
    public Guid StockId { get; set; } = default!;
    public Guid MeasurementUnitId { get; set; } = default!;
    public decimal Amount { get; set; }
}
