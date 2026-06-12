namespace Ucms.Application.Features.Estimates;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class DeleteSection
{
    public record Command(Guid ProjectId, Guid SectionId);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(bool NotFound, bool Forbidden)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var orgId = await db.Projects
                .Where(p => p.Id == cmd.ProjectId && !p.IsDeleted)
                .Select(p => (Guid?)p.OrganizationId)
                .FirstOrDefaultAsync(ct);

            if (orgId is null || (!ctx.IsOwner && ctx.OrganizationId != orgId))
                return (false, orgId is not null);

            var section = await db.EstimateSections
                .FirstOrDefaultAsync(s => s.Id == cmd.SectionId && s.ProjectId == cmd.ProjectId, ct);

            if (section is null) return (true, false);

            db.EstimateSections.Remove(section);
            await db.SaveChangesAsync(ct);
            return (false, false);
        }
    }
}
