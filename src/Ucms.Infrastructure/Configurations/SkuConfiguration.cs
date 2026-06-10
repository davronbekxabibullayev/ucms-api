namespace Ucms.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Infrastructure.Configurations.Base;
using Ucms.Domain.Entities;

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
