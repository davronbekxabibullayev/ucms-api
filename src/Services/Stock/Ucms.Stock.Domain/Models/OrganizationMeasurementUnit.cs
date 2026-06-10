namespace Ucms.Stock.Domain.Models;

using Ucms.Stock.Domain.Models.Entities;
using Ucms.Stock.Domain.Models.Enums;

/// <summary>
/// Стандартная единица измерения организации
/// </summary>
public class OrganizationMeasurementUnit : Entity, IDeletable
{
    /// <summary>
    /// Тип единицы измерение
    /// </summary>
    public MeasurementUnitType Type { get; set; }

    /// <summary>
    /// Удален или нет
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Идентификатор единицы измерение
    /// </summary>
    public Guid MeasurementUnitId { get; set; }

    /// <summary>
    /// Идентификатор организации
    /// </summary>
    public Guid OrganizationId { get; set; }

    public virtual MeasurementUnit? MeasurementUnit { get; set; }
}
