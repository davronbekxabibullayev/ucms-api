namespace Ucms.Application.Features.Skus;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;

public static class DeleteSku
{
    public record Command(Guid Id);

    public sealed class Handler(IUcmsDbContext db)
    {
        public async Task<(bool NotFound, string? Error)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var sku = await db.Skus.AsTracking().FirstOrDefaultAsync(a => a.Id == cmd.Id, ct);
            if (sku is null) return (true, null);

            if (db.IncomeItems.Any(a => a.SkuId == cmd.Id) ||
                db.OutcomeItems.Any(a => a.SkuId == cmd.Id) ||
                db.StockSkus.Any(a => a.SkuId == cmd.Id))
                return (false, "SKU boshqa jadvallarda ishlatilmoqda");

            sku.IsDeleted = true;
            await db.SaveChangesAsync(ct);
            return (false, null);
        }
    }
}
