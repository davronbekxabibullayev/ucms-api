namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class IncomeItemConfiguration : IEntityTypeConfiguration<IncomeItem>
{
    public void Configure(EntityTypeBuilder<IncomeItem> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("IncomeId").IsRequired();
        builder.Property("SkuId").IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
