namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class IncomeOutcomeConfiguration : IEntityTypeConfiguration<IncomeOutcome>
{
    public void Configure(EntityTypeBuilder<IncomeOutcome> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
