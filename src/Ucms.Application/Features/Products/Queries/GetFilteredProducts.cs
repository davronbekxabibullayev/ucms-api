namespace Ucms.Application.Features.Products;

using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Extensions;
using QueryForge.Models;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;

public static class GetFilteredProducts
{
    public record Query(PagedRequest Filter);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<PagedResult<ProductModel>> HandleAsync(Query q, CancellationToken ct) =>
            await db.Products.OrderBy(x => x.Name)
                .ToPagedResultAsync<Product, ProductModel>(q.Filter, mapper, ct);
    }
}
