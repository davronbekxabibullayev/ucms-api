namespace Ucms.Application.Features.Salaries.Queries;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;

public static class GetSalaries
{
    public record Query(string? Month, int Page, int Size);

    public record Item(
        Guid Id, string EmployeeName, string? Position,
        string Month, decimal Amount, string? Notes, DateTimeOffset CreatedAt);

    public record Result(int Total, int Page, int Size, decimal TotalAmount, List<Item> Items);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(Result? Data, bool Forbidden)> HandleAsync(Query q, CancellationToken ct)
        {
            if (!ctx.IsOwner && !ctx.OrganizationId.HasValue) return (null, true);

            var query = db.Salaries.Where(s => !s.IsDeleted);

            if (!ctx.IsOwner && ctx.OrganizationId.HasValue)
                query = query.Where(s => s.OrganizationId == ctx.OrganizationId.Value);

            if (!string.IsNullOrEmpty(q.Month))
                query = query.Where(s => s.Month == q.Month);

            var total = await query.CountAsync(ct);
            var totalAmount = await query.SumAsync(s => s.Amount, ct);

            var items = await query
                .OrderByDescending(s => s.Month)
                .ThenBy(s => s.EmployeeName)
                .Skip((q.Page - 1) * q.Size).Take(q.Size)
                .Select(s => new Item(
                    s.Id, s.EmployeeName, s.Position,
                    s.Month, s.Amount, s.Notes, s.CreatedAt))
                .ToListAsync(ct);

            return (new Result(total, q.Page, q.Size, totalAmount, items), false);
        }
    }
}
