namespace Ucms.Application.Features.WorkLogs;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class GetWorkLogById
{
    public record Query(Guid ProjectId, Guid Id);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(object? Data, bool ProjectNotFound, bool Forbidden)> HandleAsync(Query q, CancellationToken ct)
        {
            var orgId = await db.Projects
                .Where(p => p.Id == q.ProjectId && !p.IsDeleted)
                .Select(p => (Guid?)p.OrganizationId)
                .FirstOrDefaultAsync(ct);

            if (orgId is null) return (null, true, false);
            if (!ctx.IsOwner && ctx.OrganizationId != orgId) return (null, false, true);

            var workLog = await db.WorkLogs
                .Where(w => w.Id == q.Id && w.ProjectId == q.ProjectId)
                .Select(w => (object)new
                {
                    w.Id, w.Date, w.Volume, w.BrigadeUnitPrice, w.TotalAmount,
                    w.Status, w.Note, w.BrigadePaymentId,
                    w.CreatedAt, w.UpdatedAt,
                    Brigade      = new { w.Brigade!.Id, w.Brigade.Name },
                    EstimateItem = new { w.EstimateItem!.Id, w.EstimateItem.Name, w.EstimateItem.Unit },
                })
                .FirstOrDefaultAsync(ct);

            return (workLog, false, false);
        }
    }
}
