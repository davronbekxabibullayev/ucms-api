namespace Ucms.Application.Features.ClientActs;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class GetClientActById
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

            var act = await db.ClientActs
                .Where(a => a.Id == q.Id && a.ProjectId == q.ProjectId)
                .Select(a => (object)new
                {
                    a.Id, a.ActNumber, a.ActDate, a.TotalAmount, a.Status, a.Note,
                    a.CreatedAt, a.UpdatedAt,
                    Items = a.Items.Select(i => new
                    {
                        i.Id, i.EstimateItemId,
                        ItemName = i.EstimateItem!.Name,
                        Unit     = i.EstimateItem.Unit,
                        i.Volume, i.UnitPrice, i.TotalAmount,
                    }),
                    Payments = a.Payments.Select(p => new
                    {
                        p.Id, p.Date, p.Amount, p.PaymentMethod, p.Note,
                    }),
                    PaidAmount = a.Payments.Sum(p => p.Amount),
                })
                .FirstOrDefaultAsync(ct);

            return (act, false, false);
        }
    }
}
