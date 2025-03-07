namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations.Configuration.EntityConfiguration;
using Ucms.Stock.Domain.Models;

public class ProductConfiguration : LocalizableConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.Property("Code").HasMaxLength(32).IsRequired(false);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

