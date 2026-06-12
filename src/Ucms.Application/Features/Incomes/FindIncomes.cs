namespace Ucms.Application.Features.Incomes;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;

public static class FindIncomes
{
    public record Query(string Search);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<List<IncomeModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var s = q.Search.ToLower();
            var list = await db.Incomes
                .Where(a => a.Name!.ToLower().Contains(s) || (a.Note != null && a.Note.ToLower().Contains(s)))
                .ToListAsync(ct);
            return mapper.Map<List<IncomeModel>>(list);
        }
    }
}
