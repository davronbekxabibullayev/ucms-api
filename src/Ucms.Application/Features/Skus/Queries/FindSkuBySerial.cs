namespace Ucms.Application.Features.Skus;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;

public static class FindSkuBySerial
{
    public record Query(string SerialNumber);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<SkuModel?> HandleAsync(Query q, CancellationToken ct)
        {
            var sku = await db.Skus.FirstOrDefaultAsync(f => f.SerialNumber == q.SerialNumber, ct);
            return sku is null ? null : mapper.Map<SkuModel>(sku);
        }
    }
}
