namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations.Configuration.EntityConfiguration;
using Ucms.Stock.Domain.Models;

public class SupplierConfiguration : LocalizableConfiguration<Supplier>
{
    public override void Configure(EntityTypeBuilder<Supplier> builder)
    {
        base.Configure(builder);

        builder.Property("Code").HasMaxLength(32).IsRequired(true);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

