namespace Ucms.Application.Features.Payments;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

public static class CreateBrigadePayment
{
    public record Command(
        Guid ProjectId,
        Guid BrigadeId,
        DateTimeOffset Date,
        decimal Amount,
        PaymentMethod PaymentMethod,
        Guid[] WorkLogIds,
        string? Note);

    public record Result(Guid Id, decimal Amount);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(Result? Data, bool ProjectNotFound, bool Forbidden)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var orgId = await db.Projects
                .Where(p => p.Id == cmd.ProjectId && !p.IsDeleted)
                .Select(p => (Guid?)p.OrganizationId)
                .FirstOrDefaultAsync(ct);

            if (orgId is null) return (null, true, false);
            if (!ctx.IsOwner && ctx.OrganizationId != orgId) return (null, false, true);

            var now       = DateTimeOffset.UtcNow;
            var userId    = ctx.UserId ?? Guid.Empty;
            var paymentId = Guid.NewGuid();

            var payment = new BrigadePayment
            {
                Id            = paymentId,
                ProjectId     = cmd.ProjectId,
                BrigadeId     = cmd.BrigadeId,
                Date          = cmd.Date,
                Amount        = cmd.Amount,
                PaymentMethod = cmd.PaymentMethod,
                Note          = cmd.Note,
                CreatedAt     = now, UpdatedAt = now,
                CreatedBy     = userId, UpdatedBy = userId,
            };

            await db.BrigadePayments.AddAsync(payment, ct);

            if (cmd.WorkLogIds.Length > 0)
            {
                var workLogs = await db.WorkLogs
                    .Where(w => cmd.WorkLogIds.Contains(w.Id)
                             && w.ProjectId == cmd.ProjectId
                             && w.BrigadeId == cmd.BrigadeId
                             && w.Status == WorkLogStatus.Confirmed)
                    .ToListAsync(ct);

                foreach (var wl in workLogs)
                {
                    wl.Status           = WorkLogStatus.Paid;
                    wl.BrigadePaymentId = paymentId;
                    wl.UpdatedAt        = now;
                    wl.UpdatedBy        = userId;
                    db.WorkLogs.Update(wl);
                }
            }

            await db.SaveChangesAsync(ct);
            return (new Result(payment.Id, payment.Amount), false, false);
        }
    }
}
