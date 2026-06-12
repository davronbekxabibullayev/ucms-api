namespace Ucms.Application.Features.Skus.Queries;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Features.Skus.DTOs;
using Ucms.Application.Persistence;

public static class GetSkuById
{
    public record Query(Guid Id);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<SkuModel?> HandleAsync(Query q, CancellationToken ct)
        {
            var sku = await db.Skus.FirstOrDefaultAsync(f => f.Id == q.Id, ct);
            return sku is null ? null : mapper.Map<SkuModel>(sku);
        }
    }
}
