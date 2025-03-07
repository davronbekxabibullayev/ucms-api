namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class OutcomeItemConfiguration : IEntityTypeConfiguration<OutcomeItem>
{
    public void Configure(EntityTypeBuilder<OutcomeItem> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("OutcomeId").IsRequired();
        builder.Property("SkuId").IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
