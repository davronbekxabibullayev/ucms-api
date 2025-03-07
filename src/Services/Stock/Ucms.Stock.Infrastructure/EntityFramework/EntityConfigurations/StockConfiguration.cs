namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations.Configuration.EntityConfiguration;

public class StockConfiguration : LocalizableConfiguration<Domain.Models.Stock>
{
    public override void Configure(EntityTypeBuilder<Domain.Models.Stock> builder)
    {
        base.Configure(builder);
        builder.Property("Code").HasMaxLength(256).IsRequired();
        builder.Property("OrganizationId").IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
