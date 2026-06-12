namespace Ucms.Application.Features.Skus;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

public static class CreateSku
{
    public record Command(
        string Name, string NameRu, string? NameEn, string? NameKa,
        string SerialNumber, Guid ProductId, Guid? ManufacturerId,
        Guid MeasurementUnitId, Guid? SupplierId,
        decimal Price, decimal Amount, DateTimeOffset ExpirationDate, SkuStatus Status);

    public sealed class Handler(IUcmsDbContext db, IWorkContext workContext)
    {
        public async Task<(Guid? Id, string? Error)> HandleAsync(Command cmd, CancellationToken ct)
        {
            if (await db.Skus.AnyAsync(f => f.SerialNumber == cmd.SerialNumber, ct))
                return (null, $"'{cmd.SerialNumber}' seriya raqami allaqachon mavjud");

            var orgSku = new OrganizationSku
            {
                OrganizationId = workContext.TenantId!.Value,
                Sku = new Sku
                {
                    Name = cmd.Name, NameRu = cmd.NameRu, NameEn = cmd.NameEn, NameKa = cmd.NameKa,
                    SerialNumber = cmd.SerialNumber, Price = cmd.Price, Amount = cmd.Amount,
                    ExpirationDate = cmd.ExpirationDate, ProductId = cmd.ProductId,
                    ManufacturerId = cmd.ManufacturerId, MeasurementUnitId = cmd.MeasurementUnitId,
                    SupplierId = cmd.SupplierId, Status = cmd.Status
                }
            };
            db.OrganizationSkus.Add(orgSku);
            await db.SaveChangesAsync(ct);
            return (orgSku.SkuId, null);
        }
    }
}
