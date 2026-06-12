namespace Ucms.Application.Features.ProjectExpenses.Commands;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class UpdateProjectExpense
{
    public record Command(
        Guid ProjectId, Guid Id, DateTimeOffset Date, string Category,
        decimal Amount, string? Description, string? PaymentMethod, string? Note);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(bool NotFound, bool Forbidden)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var expense = await db.ProjectExpenses
                .FirstOrDefaultAsync(e => e.Id == cmd.Id && e.ProjectId == cmd.ProjectId && !e.IsDeleted, ct);

            if (expense is null) return (true, false);

            if (!ctx.IsOwner && ctx.OrganizationId != expense.OrganizationId) return (false, true);

            expense.Date          = cmd.Date;
            expense.Category      = cmd.Category;
            expense.Amount        = cmd.Amount;
            expense.Description   = cmd.Description;
            expense.PaymentMethod = cmd.PaymentMethod;
            expense.Note          = cmd.Note;
            expense.UpdatedAt     = DateTimeOffset.UtcNow;
            expense.UpdatedBy     = ctx.UserId ?? Guid.Empty;

            db.ProjectExpenses.Update(expense);
            await db.SaveChangesAsync(ct);
            return (false, false);
        }
    }
}
