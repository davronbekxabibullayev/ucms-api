namespace Ucms.Application.Features.Skus.Commands;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;

public static class DeleteSkus
{
    public record Command(Guid[] Ids);

    public sealed class Handler(IUcmsDbContext db)
    {
        public async Task<(int Deleted, string? Error)> HandleAsync(Command cmd, CancellationToken ct)
        {
            if (db.IncomeItems.Any(a => cmd.Ids.Contains(a.SkuId)) ||
                db.OutcomeItems.Any(a => cmd.Ids.Contains(a.SkuId)) ||
                db.StockSkus.Any(a => cmd.Ids.Contains(a.SkuId)))
                return (0, "SKUlar boshqa jadvallarda ishlatilmoqda");

            var skus = await db.Skus.AsTracking()
                .Where(f => cmd.Ids.Contains(f.Id)).ToListAsync(ct);
            foreach (var s in skus) s.IsDeleted = true;
            await db.SaveChangesAsync(ct);
            return (skus.Count, null);
        }
    }
}
