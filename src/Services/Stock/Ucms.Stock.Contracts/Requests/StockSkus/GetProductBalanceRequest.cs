namespace Ucms.Stock.Contracts.Requests.StockSkus;

public class GetProductBalanceRequest
{
    public Guid StockId { get; set; } = default!;
    public Guid ProductId { get; set; } = default!;
    public Guid MeasurementUnitId { get; set; } = default!;
}
