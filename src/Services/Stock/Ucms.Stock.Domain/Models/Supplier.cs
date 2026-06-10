namespace Ucms.Stock.Domain.Models;

using Ucms.Stock.Domain.Models.Entities;

/// <summary>
/// Поставщик
/// </summary>
public class Supplier : LocalizableEntity, IDeletable
{
    /// <summary>
    /// Код поставщика
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Удален или нет
    /// </summary>
    public bool IsDeleted { get; set; }
}
