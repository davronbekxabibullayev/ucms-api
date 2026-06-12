namespace Ucms.Application.Features.Skus.Queries;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.Abstractions;
using Ucms.Application.Features.Skus.DTOs;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

public static class GetProductStockSkus
{
    public record Query(PagedRequest Paging, Guid? ProductId, Guid? StockId, List<ProductType>? Types, string? Search);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper, IWorkContext workContext)
    {
        public async Task<List<SkuModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var query = db.StockSkus
                .Include(i => i.Sku!.Product)
                .Include(i => i.Sku!.MeasurementUnit)
                .Where(w => w.Amount > 0 && w.Stock!.OrganizationId == workContext.TenantId);

            if (q.ProductId.HasValue) query = query.Where(w => w.Sku!.ProductId == q.ProductId);
            if (q.StockId.HasValue) query = query.Where(w => w.StockId == q.StockId);
            if (q.Types is { Count: > 0 }) query = query.Where(w => q.Types.Contains(w.Sku!.Product!.Type));
            if (!string.IsNullOrEmpty(q.Search))
            {
                var name = q.Search.ToLower();
                query = query.Where(w =>
                    w.Sku!.Name.ToLower().Contains(name) || w.Sku!.NameEn!.ToLower().Contains(name) ||
                    w.Sku!.NameRu.ToLower().Contains(name) || w.Sku!.NameKa!.ToLower().Contains(name) ||
                    w.Sku!.SerialNumber.Contains(name));
            }

            var stockSkus = await query
                .OrderBy(o => o.Sku!.Name)
                .Skip(q.Paging.Page).Take(q.Paging.PageSize)
                .ToListAsync(ct);

            return stockSkus.Select(s =>
            {
                var sku = mapper.Map<SkuModel>(s.Sku);
                sku.StockSkuAmount = s.Amount;
                return sku;
            }).ToList();
        }
    }
}
