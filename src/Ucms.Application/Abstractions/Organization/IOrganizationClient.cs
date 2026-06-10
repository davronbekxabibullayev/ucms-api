namespace Ucms.Application.Abstractions.Organization;

public interface IOrganizationClient
{
    Task<IEnumerable<Guid?>> GetOrganizationIds(Guid? organizationId = null, bool includeChilds = false, Guid? regionId = null, Guid? cityId = null);
    Task<IEnumerable<Guid>> GetStockIds(Guid? organizationId = null);
    Task<bool> CheckOrganizationBrigadeStock(Guid stockId);
}
