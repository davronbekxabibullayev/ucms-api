namespace Ucms.Application.Features.Skus.Commands;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;

public static class UpdateSku
{
    public record Command(
        Guid Id, string Name, string NameRu, string? NameEn, string? NameKa,
        string SerialNumber, Guid ProductId, Guid? ManufacturerId,
        Guid MeasurementUnitId, Guid? SupplierId,
        decimal Price, decimal Amount, DateTimeOffset ExpirationDate, SkuStatus Status);

    public sealed class Handler(IUcmsDbContext db)
    {
        public async Task<bool> HandleAsync(Command cmd, CancellationToken ct)
        {
            var sku = await db.Skus.AsTracking().FirstOrDefaultAsync(f => f.Id == cmd.Id, ct);
            if (sku is null) return false;
            sku.Name = cmd.Name; sku.NameRu = cmd.NameRu; sku.NameEn = cmd.NameEn; sku.NameKa = cmd.NameKa;
            sku.SerialNumber = cmd.SerialNumber; sku.Price = cmd.Price; sku.Amount = cmd.Amount;
            sku.ExpirationDate = cmd.ExpirationDate; sku.ProductId = cmd.ProductId;
            sku.ManufacturerId = cmd.ManufacturerId; sku.MeasurementUnitId = cmd.MeasurementUnitId;
            sku.SupplierId = cmd.SupplierId; sku.Status = cmd.Status;
            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}
