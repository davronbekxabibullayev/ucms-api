namespace Ucms.Application.Features.StockDemands;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;

public static class GetStockDemands
{
    public record Query;

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<List<StockDemandModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var list = await db.StockDemands.OrderBy(a => a.Name).ToListAsync(ct);
            return mapper.Map<List<StockDemandModel>>(list);
        }
    }
}
