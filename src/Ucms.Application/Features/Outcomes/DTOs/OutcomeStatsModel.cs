namespace Ucms.Application.Features.Outcomes;

using Ucms.Application.Features.Skus;

public record OutcomeStatsModel(
    List<OutcomeStatItemModel> CurrentPeriod,
    List<OutcomeStatItemModel> PreviousPeriod
);

public record OutcomeStatItemModel(
    SkuModel Sku,
    int Count
);
