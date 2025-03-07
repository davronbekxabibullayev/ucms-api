namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations.Configuration.EntityConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models.Entities;

/// <summary>
/// Базовый класс конфигурации для сущностей людей
/// </summary>
/// <typeparam name="TEntity">Класс производный от PersonBase</typeparam>
public abstract class PersonBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : PersonBase
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasIndex(e => e.Id);
        builder.Property("FirstName").HasMaxLength(256).IsRequired();
        builder.Property("LastName").HasMaxLength(256).IsRequired();
        builder.Property("MiddleName").HasMaxLength(256).IsRequired(false);
    }
}
