namespace Ucms.Stock.Contracts.Models;

public record ReceivedDemandModel : StockDemandModel
{
    public string StockName { get; set; } = default!;
    public string StockNameRu { get; set; } = default!;
    public string StockNameEn { get; set; } = default!;
    public string StockNameKa { get; set; } = default!;
}
