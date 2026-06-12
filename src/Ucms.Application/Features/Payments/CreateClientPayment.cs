namespace Ucms.Application.Features.Payments;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

public static class CreateClientPayment
{
    public record Command(
        Guid ProjectId,
        Guid? ActId,
        DateTimeOffset Date,
        decimal Amount,
        PaymentMethod PaymentMethod,
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

            var now    = DateTimeOffset.UtcNow;
            var userId = ctx.UserId ?? Guid.Empty;

            var payment = new ClientPayment
            {
                Id            = Guid.NewGuid(),
                ProjectId     = cmd.ProjectId,
                ActId         = cmd.ActId,
                Date          = cmd.Date,
                Amount        = cmd.Amount,
                PaymentMethod = cmd.PaymentMethod,
                Note          = cmd.Note,
                CreatedAt     = now, UpdatedAt = now,
                CreatedBy     = userId, UpdatedBy = userId,
            };

            await db.ClientPayments.AddAsync(payment, ct);

            if (cmd.ActId.HasValue)
                await UpdateActStatusAsync(cmd.ActId.Value, ct);

            await db.SaveChangesAsync(ct);
            return (new Result(payment.Id, payment.Amount), false, false);
        }

        private async Task UpdateActStatusAsync(Guid actId, CancellationToken ct)
        {
            var act = await db.ClientActs
                .Include(a => a.Payments)
                .FirstOrDefaultAsync(a => a.Id == actId, ct);

            if (act is null) return;

            var paid = act.Payments.Sum(p => p.Amount);
            act.Status = paid >= act.TotalAmount
                ? ActStatus.PaidFully
                : paid > 0 ? ActStatus.PaidPartially : ActStatus.Issued;

            db.ClientActs.Update(act);
        }
    }
}
