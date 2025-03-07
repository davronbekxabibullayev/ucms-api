namespace Ucms.Stock.Contracts.Requests.Stats;

public record GetStocksStatisticsRequest(
    Guid? OrganizationId,
    Guid? RegionId,
    Guid? CityId);
