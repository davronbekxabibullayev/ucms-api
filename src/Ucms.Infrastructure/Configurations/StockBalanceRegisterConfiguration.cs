namespace Ucms.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Domain.Entities;

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
