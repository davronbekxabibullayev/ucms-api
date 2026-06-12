namespace Ucms.Application.Features.Projects.Queries;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Features.Projects.DTOs;
using Ucms.Application.Persistence;

public static class GetProjectById
{
    public record Query(Guid Id);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(ProjectDetailDto? Data, bool Forbidden)> HandleAsync(Query q, CancellationToken ct)
        {
            var project = await db.Projects
                .Where(p => p.Id == q.Id && !p.IsDeleted)
                .Select(p => new ProjectDetailDto(
                    p.Id,
                    p.Name,
                    p.Address,
                    p.Description,
                    p.ContractNumber,
                    p.ContractDate,
                    p.StartDate,
                    p.EndDate,
                    p.Status,
                    p.OrganizationId,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.EstimateSections.OrderBy(s => s.Order).Select(s => new ProjectSectionDto(
                        s.Id,
                        s.Name,
                        s.Order,
                        s.EstimateItems.OrderBy(i => i.Order).Select(i => new ProjectEstimateItemDto(
                            i.Id,
                            i.Name,
                            i.MeasurementUnit!.Code,
                            i.Volume,
                            i.ClientUnitPrice,
                            i.BrigadeUnitPrice,
                            i.Volume * i.ClientUnitPrice,
                            i.Volume * i.BrigadeUnitPrice,
                            i.Order))))))
                .FirstOrDefaultAsync(ct);

            if (project is null) return (null, false);
            if (!ctx.IsOwner && ctx.OrganizationId != project.OrganizationId) return (null, true);
            return (project, false);
        }
    }
}
