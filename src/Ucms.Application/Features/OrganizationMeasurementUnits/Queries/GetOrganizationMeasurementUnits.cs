namespace Ucms.Application.Features.OrganizationMeasurementUnits;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class GetOrganizationMeasurementUnits
{
    public record Query;

    public sealed class Handler(IUcmsDbContext db, IMapper mapper, IWorkContext workContext)
    {
        public async Task<List<OrganizationMeasurementUnitModel>> HandleAsync(Query q, CancellationToken ct)
            => mapper.Map<List<OrganizationMeasurementUnitModel>>(
                await db.OrganizationMeasurementUnits.Include(i => i.MeasurementUnit)
                    .Where(w => w.OrganizationId == workContext.TenantId).ToListAsync(ct));
    }
}
