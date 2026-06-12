namespace Ucms.Application.Features.Projects;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;

public static class GetProjectById
{
    public record Query(Guid Id);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(object? Data, bool Forbidden)> HandleAsync(Query q, CancellationToken ct)
        {
            var project = await db.Projects
                .Where(p => p.Id == q.Id && !p.IsDeleted)
                .Select(p => new
                {
                    p.Id, p.Name, p.Address, p.Description,
                    p.ContractNumber, p.ContractDate,
                    p.StartDate, p.EndDate, p.Status,
                    p.OrganizationId, p.CreatedAt, p.UpdatedAt,
                    Sections = p.EstimateSections.OrderBy(s => s.Order).Select(s => new
                    {
                        s.Id, s.Name, s.Order,
                        Items = s.EstimateItems.OrderBy(i => i.Order).Select(i => new
                        {
                            i.Id, i.Name, i.Unit, i.Volume,
                            i.ClientUnitPrice, i.BrigadeUnitPrice,
                            ClientTotal  = i.Volume * i.ClientUnitPrice,
                            BrigadeTotal = i.Volume * i.BrigadeUnitPrice,
                            i.Order,
                        }),
                    }),
                })
                .FirstOrDefaultAsync(ct);

            if (project is null) return (null, false);
            if (!ctx.IsOwner && ctx.OrganizationId != project.OrganizationId) return (null, true);
            return (project, false);
        }
    }
}
