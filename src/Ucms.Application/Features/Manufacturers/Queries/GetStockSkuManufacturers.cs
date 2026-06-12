namespace Ucms.Application.Features.Manufacturers.Queries;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Extensions;
using QueryForge.Models;
using Ucms.Application.Features.Manufacturers.DTOs;
using Ucms.Application.Persistence;

public static class GetStockSkuManufacturers
{
    public record Query(string? Search, Guid? OrganizationId, Guid? StockId, Guid? ProductId, int Page = 1, int Size = 20);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<PagedResult<ManufacturerModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var manufacturerIds = db.StockSkus
                .Where(ss =>
                    (!q.OrganizationId.HasValue || ss.Stock.OrganizationId == q.OrganizationId) &&
                    (!q.StockId.HasValue || ss.StockId == q.StockId) &&
                    (!q.ProductId.HasValue || ss.Sku.ProductId == q.ProductId))
                .Select(ss => ss.Sku.ManufacturerId)
                .Where(id => id != null)
                .Distinct();

            var query = db.Manufacturers.Where(m => manufacturerIds.Contains(m.Id)).OrderBy(x => x.Name);
            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.ToLower().Trim();
                query = (IOrderedQueryable<Domain.Entities.Manufacturer>)query.Where(x =>
                    x.Name.ToLower().Contains(s) || x.NameRu.ToLower().Contains(s) ||
                    (x.NameKa != null && x.NameKa.ToLower().Contains(s)) ||
                    (x.NameEn != null && x.NameEn.ToLower().Contains(s)));
            }
            var paged = new PagedRequest { Page = q.Page, PageSize = q.Size };
            return await query.ToPagedResultAsync<Domain.Entities.Manufacturer, ManufacturerModel>(paged, mapper, ct);
        }
    }
}
