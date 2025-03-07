namespace Ucms.Stock.Domain.Models;

using Ucms.Stock.Domain.Models.Entities;
using Ucms.Stock.Domain.Models.Enums;

/// <summary>
/// Единица складского учета
/// </summary>
public class Sku : LocalizableEntity, IDeletable
{
    /// <summary>
    /// Сериа номер Единицы складского учета
    /// </summary>
    public string SerialNumber { get; set; } = default!;

    /// <summary>
    /// Срок годности
    /// </summary>
    public DateTimeOffset ExpirationDate { get; set; }

    /// <summary>
    /// Удален или нет
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public Guid ProductId { get; set; } = default!;

    /// <summary>
    /// Идентификатор производителя
    /// </summary>
    public Guid? ManufacturerId { get; set; } = default!;

    /// <summary>
    /// Идентификатор единицы измерение
    /// </summary>
    public Guid MeasurementUnitId { get; set; } = default!;

    /// <summary>
    /// Идентификатор поставщика
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Количество Единицы складского учета
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Тип служба
    /// </summary>
    public EmergencyServiceType EmergencyType { get; set; } = EmergencyServiceType.Ambulance;

    /// <summary>
    /// Статус
    /// </summary>
    public SkuStatus Status { get; set; } = SkuStatus.Default;

    public virtual Product? Product { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual MeasurementUnit? MeasurementUnit { get; set; }

    public virtual Supplier? Supplier { get; set; }

    public virtual ICollection<StockBalanceRegister> StockBalanceRegistery { get; set; } = [];
}
