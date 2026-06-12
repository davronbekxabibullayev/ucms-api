namespace Ucms.Application.Features.WorkLogs;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;

public static class GetWorkLogs
{
    public record Query(
        Guid ProjectId,
        Guid? BrigadeId,
        WorkLogStatus? Status,
        DateTimeOffset? From,
        DateTimeOffset? To,
        int Page,
        int Size);

    public record Result(int Total, int Page, int Size, List<object> Items);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(Result? Data, bool ProjectNotFound, bool Forbidden)> HandleAsync(Query q, CancellationToken ct)
        {
            var orgId = await db.Projects
                .Where(p => p.Id == q.ProjectId && !p.IsDeleted)
                .Select(p => (Guid?)p.OrganizationId)
                .FirstOrDefaultAsync(ct);

            if (orgId is null) return (null, true, false);
            if (!ctx.IsOwner && ctx.OrganizationId != orgId) return (null, false, true);

            var query = db.WorkLogs.Where(w => w.ProjectId == q.ProjectId);

            if (q.BrigadeId.HasValue) query = query.Where(w => w.BrigadeId == q.BrigadeId.Value);
            if (q.Status.HasValue)    query = query.Where(w => w.Status == q.Status.Value);
            if (q.From.HasValue)      query = query.Where(w => w.Date >= q.From.Value);
            if (q.To.HasValue)        query = query.Where(w => w.Date <= q.To.Value);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(w => w.Date)
                .Skip((q.Page - 1) * q.Size).Take(q.Size)
                .Select(w => (object)new
                {
                    w.Id, w.Date, w.Volume, w.BrigadeUnitPrice, w.TotalAmount,
                    w.Status, w.Note, w.BrigadePaymentId,
                    Brigade      = w.Brigade!.Name,
                    EstimateItem = new { w.EstimateItem!.Name, w.EstimateItem.Unit },
                })
                .ToListAsync(ct);

            return (new Result(total, q.Page, q.Size, items), false, false);
        }
    }
}
