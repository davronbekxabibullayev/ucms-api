namespace Ucms.Application.DTOs.Requests.Stats;

public record GetStocksStatisticsRequest(
    Guid? OrganizationId,
    Guid? RegionId,
    Guid? CityId);
