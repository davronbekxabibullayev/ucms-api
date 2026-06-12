namespace Ucms.Application.Features.Skus;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;

public static class GetSkus
{
    public record Query;

    public sealed class Handler(IUcmsDbContext db, IMapper mapper, IWorkContext workContext)
    {
        public async Task<List<SkuModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var skus = await db.OrganizationSkus
                .Where(w => w.OrganizationId == workContext.TenantId)
                .OrderBy(a => a.Sku!.Name)
                .Select(s => s.Sku)
                .ToListAsync(ct);
            return mapper.Map<List<SkuModel>>(skus);
        }
    }
}
