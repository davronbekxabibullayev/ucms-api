namespace Ucms.Application.Features.Stocks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;

public static class GetStockById
{
    public record Query(Guid Id);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<StockModel?> HandleAsync(Query q, CancellationToken ct)
        {
            var stock = await db.Stocks.FirstOrDefaultAsync(f => f.Id == q.Id, ct);
            return stock is null ? null : mapper.Map<StockModel>(stock);
        }
    }
}
