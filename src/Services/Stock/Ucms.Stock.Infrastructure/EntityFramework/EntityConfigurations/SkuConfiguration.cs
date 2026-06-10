namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations.Configuration.EntityConfiguration;
using Ucms.Stock.Domain.Models;

public class SkuConfiguration : LocalizableConfiguration<Sku>
{
    public override void Configure(EntityTypeBuilder<Sku> builder)
    {
        base.Configure(builder);
        builder.Property("SerialNumber").HasMaxLength(256).IsRequired();
        builder.Property("ProductId").IsRequired();
        builder.Property("MeasurementUnitId").IsRequired();
        builder.Property("ManufacturerId").IsRequired(false);
        builder.Property("SupplierId").IsRequired(false);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
