namespace Ucms.Application.Features.Salaries.Commands;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class UpdateSalary
{
    public record Command(Guid Id, Guid EmployeeId, string Month, decimal Amount, string? Notes);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(bool NotFound, bool Forbidden, string? Error)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var salary = await db.Salaries.FindAsync([cmd.Id], ct);
            if (salary is null || salary.IsDeleted) return (true, false, null);
            if (!ctx.IsOwner && ctx.OrganizationId != salary.OrganizationId) return (false, true, null);

            // Yangi xodim tekshirish (agar o'zgartirilsa)
            if (salary.EmployeeId != cmd.EmployeeId)
            {
                var exists = await db.Employees
                    .AnyAsync(e => e.Id == cmd.EmployeeId && !e.IsDeleted, ct);
                if (!exists) return (false, false, "Xodim topilmadi");
            }

            salary.EmployeeId = cmd.EmployeeId;
            salary.Month      = cmd.Month;
            salary.Amount     = cmd.Amount;
            salary.Notes      = cmd.Notes;
            salary.UpdatedAt  = DateTimeOffset.UtcNow;
            salary.UpdatedBy  = ctx.UserId ?? Guid.Empty;

            db.Salaries.Update(salary);
            await db.SaveChangesAsync(ct);
            return (false, false, null);
        }
    }
}
