namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class StockBalanceRegisterConfiguration : IEntityTypeConfiguration<StockBalanceRegister>
{
    public void Configure(EntityTypeBuilder<StockBalanceRegister> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("StockId").IsRequired();
        builder.Property("SkuId").IsRequired();
        builder.Property("ProductId").IsRequired();
        builder.Property("MeasurementUnitId").IsRequired();
    }
}
