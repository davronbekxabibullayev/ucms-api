namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models;

public class OrganizationSkuConfiguration : IEntityTypeConfiguration<OrganizationSku>
{
    public void Configure(EntityTypeBuilder<OrganizationSku> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("SkuId").IsRequired();
        builder.Property("OrganizationId").IsRequired();
    }
}
