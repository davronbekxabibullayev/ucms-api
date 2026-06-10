namespace Ucms.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Infrastructure.Configurations.Base;

public class StockConfiguration : LocalizableConfiguration<Domain.Entities.Stock>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.Stock> builder)
    {
        base.Configure(builder);
        builder.Property("Code").HasMaxLength(256).IsRequired();
        builder.Property("OrganizationId").IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
