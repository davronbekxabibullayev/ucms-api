namespace Ucms.Stock.Infrastructure.EntityFramework.EntityConfigurations.Configuration.EntityConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ucms.Stock.Domain.Models.Entities;

/// <summary>
/// Базовый класс конфигурации для сущностей поддерживающих аудит 
/// </summary>
/// <typeparam name="TEntity">Класс производный от AuditableEntity</typeparam>
public abstract class AuditableConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : AuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasIndex(e => e.Id);

        builder.Property(a => a.CreatedBy).IsRequired();
        builder.Property(a => a.UpdatedBy).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();
    }
}
