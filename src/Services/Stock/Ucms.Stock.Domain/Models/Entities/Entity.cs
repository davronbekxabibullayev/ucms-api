namespace Ucms.Stock.Domain.Models.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Базовый класс для сущностей
/// </summary>
public abstract class Entity : IEntity
{
    [Key]
    public Guid Id { get; set; }
}
