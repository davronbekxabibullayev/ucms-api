namespace Ucms.Stock.Contracts.Requests.StockDemands;

using Ucms.Stock.Domain.Models.Enums;

public record CreateStockDemandRequest
{
    public string Name { get; set; } = default!;
    public string? Note { get; set; }
    public DateTimeOffset DemandDate { get; set; }
    public StockDemandStatus DemandStatus { get; set; }
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public IEnumerable<CreateStockDemandItemModel> Items { get; set; } = [];
}

public record CreateStockDemandItemModel(Guid ProductId,
                                         Guid MeasurementUnitId,
                                         decimal Amount,
                                         string? Note,
                                         bool NotApproved);
