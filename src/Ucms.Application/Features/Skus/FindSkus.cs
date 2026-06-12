namespace Ucms.Application.Features.Skus;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;

public static class FindSkus
{
    public record Query(string Search);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<List<SkuModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var s = q.Search.ToLower();
            var skus = await db.Skus
                .Where(a =>
                    a.Name.ToLower().Contains(s) || a.NameEn!.ToLower().Contains(s) ||
                    a.NameRu.ToLower().Contains(s) || a.NameKa!.ToLower().Contains(s) ||
                    a.SerialNumber.Contains(s))
                .OrderBy(a => a.Name)
                .ToListAsync(ct);
            return mapper.Map<List<SkuModel>>(skus);
        }
    }
}
