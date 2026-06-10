namespace Ucms.Stock.Contracts.Models;

public record OutcomeStatsModel(
    List<OutcomeStatItemModel> CurrentPeriod,
    List<OutcomeStatItemModel> PreviousPeriod
);

public record OutcomeStatItemModel(
    SkuModel Sku,
    int Count
);
