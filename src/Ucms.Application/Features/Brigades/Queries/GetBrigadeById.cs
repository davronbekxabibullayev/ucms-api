namespace Ucms.Application.Features.Brigades.Queries;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class GetBrigadeById
{
    public record Query(Guid Id);

    public record BrigadeDetailDto(
        Guid Id, string Name, string? LeaderName, string? Phone,
        bool IsActive, string Status, string? Notes,
        Guid OrganizationId, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(BrigadeDetailDto? Data, bool Forbidden)> HandleAsync(Query q, CancellationToken ct)
        {
            var brigade = await db.Brigades
                .Where(b => b.Id == q.Id && !b.IsDeleted)
                .Select(b => new BrigadeDetailDto(
                    b.Id, b.Name, b.ForemanName, b.Phone,
                    b.IsActive, b.IsActive ? "active" : "archived", b.Notes,
                    b.OrganizationId, b.CreatedAt, b.UpdatedAt))
                .FirstOrDefaultAsync(ct);

            if (brigade is null) return (null, false);
            if (!ctx.IsOwner && ctx.OrganizationId != brigade.OrganizationId) return (null, true);
            return (brigade, false);
        }
    }
}
