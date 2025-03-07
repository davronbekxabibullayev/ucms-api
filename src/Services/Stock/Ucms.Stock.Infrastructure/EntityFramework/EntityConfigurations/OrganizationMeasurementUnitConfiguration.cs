namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class OrganizationMeasurementUnitConfiguration : IEntityTypeConfiguration<OrganizationMeasurementUnit>
{
    public void Configure(EntityTypeBuilder<OrganizationMeasurementUnit> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("MeasurementUnitId").IsRequired();
        builder.Property("OrganizationId").IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
