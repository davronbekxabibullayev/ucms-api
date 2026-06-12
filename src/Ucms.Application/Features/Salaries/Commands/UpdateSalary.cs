namespace Ucms.Application.Features.Salaries.Commands;

using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class UpdateSalary
{
    public record Command(
        Guid Id, string EmployeeName, string? Position,
        string Month, decimal Amount, string? Notes);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(bool NotFound, bool Forbidden)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var salary = await db.Salaries.FindAsync([cmd.Id], ct);
            if (salary is null || salary.IsDeleted) return (true, false);
            if (!ctx.IsOwner && ctx.OrganizationId != salary.OrganizationId) return (false, true);

            salary.EmployeeName = cmd.EmployeeName;
            salary.Position     = cmd.Position;
            salary.Month        = cmd.Month;
            salary.Amount       = cmd.Amount;
            salary.Notes        = cmd.Notes;
            salary.UpdatedAt    = DateTimeOffset.UtcNow;
            salary.UpdatedBy    = ctx.UserId ?? Guid.Empty;

            db.Salaries.Update(salary);
            await db.SaveChangesAsync(ct);
            return (false, false);
        }
    }
}
