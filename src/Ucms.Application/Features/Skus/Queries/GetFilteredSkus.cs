namespace Ucms.Application.Features.Skus.Queries;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Extensions;
using QueryForge.Models;
using Ucms.Application.Abstractions;
using Ucms.Application.Features.Skus.DTOs;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;

public static class GetFilteredSkus
{
    public record Query(PagedRequest Paging, string? Search, string? SerialNumber);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper, IWorkContext workContext)
    {
        public async Task<PagedResult<SkuModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var query = db.OrganizationSkus
                .Include(i => i.Sku!.Manufacturer)
                .Include(i => i.Sku!.MeasurementUnit)
                .Include(i => i.Sku!.Supplier)
                .Where(w => w.OrganizationId == workContext.TenantId)
                .Select(s => s.Sku!);

            if (!string.IsNullOrEmpty(q.Search))
            {
                var s = q.Search.ToLowerInvariant().Trim();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(s) || x.NameRu.ToLower().Contains(s) ||
                    x.NameKa!.ToLower().Contains(s) || x.NameEn!.ToLower().Contains(s));
            }
            if (!string.IsNullOrEmpty(q.SerialNumber))
            {
                var sn = q.SerialNumber.ToLowerInvariant().Trim();
                query = query.Where(w => w.SerialNumber.ToLower().Contains(sn));
            }
            return await query.ToPagedResultAsync<Sku, SkuModel>(q.Paging, mapper, ct);
        }
    }
}
