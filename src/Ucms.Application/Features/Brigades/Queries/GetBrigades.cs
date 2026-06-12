namespace Ucms.Application.Features.Brigades.Queries;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class GetBrigades
{
    public record Query(bool? IsActive);

    public record Item(
        Guid Id, string Name, string? ForemanName, string? Phone,
        bool IsActive, Guid OrganizationId, DateTimeOffset CreatedAt);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<List<Item>> HandleAsync(Query q, CancellationToken ct)
        {
            var query = db.Brigades.Where(b => !b.IsDeleted);

            if (!ctx.IsOwner && ctx.OrganizationId.HasValue)
                query = query.Where(b => b.OrganizationId == ctx.OrganizationId.Value);

            if (q.IsActive.HasValue)
                query = query.Where(b => b.IsActive == q.IsActive.Value);

            return await query
                .OrderBy(b => b.Name)
                .Select(b => new Item(b.Id, b.Name, b.ForemanName, b.Phone,
                    b.IsActive, b.OrganizationId, b.CreatedAt))
                .ToListAsync(ct);
        }
    }
}
