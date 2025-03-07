namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class StockDemandItemConfiguration : IEntityTypeConfiguration<StockDemandItem>
{
    public void Configure(EntityTypeBuilder<StockDemandItem> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("StockDemandId").IsRequired();
        builder.Property("ProductId").IsRequired();
        builder.Property("MeasurementUnitId").IsRequired();
        builder.Property("Note").HasMaxLength(1024).IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
