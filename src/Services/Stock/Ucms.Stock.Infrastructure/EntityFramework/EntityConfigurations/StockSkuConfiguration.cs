namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class StockSkuConfiguration : IEntityTypeConfiguration<StockSku>
{
    public void Configure(EntityTypeBuilder<StockSku> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("SkuId").IsRequired();
        builder.Property("StockId").IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
