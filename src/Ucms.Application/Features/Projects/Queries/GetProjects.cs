namespace Ucms.Application.Features.Projects;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;

public static class GetProjects
{
    public record Query(ProjectStatus? Status, int Page, int Size);

    public record Item(
        Guid Id, string Name, string? Address, string? ContractNumber,
        ProjectStatus Status, DateTimeOffset? StartDate, DateTimeOffset? EndDate,
        Guid OrganizationId, DateTimeOffset CreatedAt);

    public record Result(int Total, int Page, int Size, List<Item> Items);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(Result? Data, bool Forbidden)> HandleAsync(Query q, CancellationToken ct)
        {
            var orgId = ctx.IsOwner ? (Guid?)null : ctx.OrganizationId;
            if (orgId is null && !ctx.IsOwner) return (null, true);

            var query = db.Projects.Where(p => !p.IsDeleted);
            if (orgId.HasValue) query = query.Where(p => p.OrganizationId == orgId.Value);
            if (q.Status.HasValue) query = query.Where(p => p.Status == q.Status.Value);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((q.Page - 1) * q.Size).Take(q.Size)
                .Select(p => new Item(p.Id, p.Name, p.Address, p.ContractNumber,
                    p.Status, p.StartDate, p.EndDate, p.OrganizationId, p.CreatedAt))
                .ToListAsync(ct);

            return (new Result(total, q.Page, q.Size, items), false);
        }
    }
}
