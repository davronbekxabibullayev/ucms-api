namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations.Configuration.EntityConfiguration;
using Ucms.Stock.Domain.Models;

public class MeasurementUnitConfiguration : LocalizableConfiguration<MeasurementUnit>
{
    public override void Configure(EntityTypeBuilder<MeasurementUnit> builder)
    {
        base.Configure(builder);
        builder.Property("Code").HasMaxLength(32).IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
