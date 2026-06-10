namespace Ucms.Application.DTOs.Models;

public record OutcomeStatsModel(
    List<OutcomeStatItemModel> CurrentPeriod,
    List<OutcomeStatItemModel> PreviousPeriod
);

public record OutcomeStatItemModel(
    SkuModel Sku,
    int Count
);
