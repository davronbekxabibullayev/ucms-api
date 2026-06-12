namespace Ucms.Application.Features.Incomes;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;

public static class FindIncome
{
    public record Query(string Name);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<IncomeModel?> HandleAsync(Query q, CancellationToken ct)
        {
            var income = await db.Incomes.Include(i => i.IncomeItems).Include(a => a.Stock)
                .FirstOrDefaultAsync(f => f.Name == q.Name, ct);
            return income is null ? null : mapper.Map<IncomeModel>(income);
        }
    }
}
