namespace Ucms.Application.Features.Salaries.Commands;

using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;

public static class CreateSalary
{
    public record Command(
        string EmployeeName, string? Position,
        string Month, decimal Amount, string? Notes);

    public record Result(Guid Id, string EmployeeName, decimal Amount);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<Result?> HandleAsync(Command cmd, CancellationToken ct)
        {
            var orgId = ctx.OrganizationId;
            if (!orgId.HasValue) return null;

            var now    = DateTimeOffset.UtcNow;
            var userId = ctx.UserId ?? Guid.Empty;

            var salary = new Salary
            {
                Id             = Guid.NewGuid(),
                OrganizationId = orgId.Value,
                EmployeeName   = cmd.EmployeeName,
                Position       = cmd.Position,
                Month          = cmd.Month,
                Amount         = cmd.Amount,
                Notes          = cmd.Notes,
                IsDeleted      = false,
                CreatedAt      = now, UpdatedAt = now,
                CreatedBy      = userId, UpdatedBy = userId,
            };

            await db.Salaries.AddAsync(salary, ct);
            await db.SaveChangesAsync(ct);
            return new Result(salary.Id, salary.EmployeeName, salary.Amount);
        }
    }
}
