namespace Ucms.Stock.Domain.Models;

using Ucms.Stock.Domain.Models.Entities;

/// <summary>
/// Единица складского учета организация
/// </summary>
public class OrganizationSku : Entity
{
    /// <summary>
    /// Идентификатор организация
    /// </summary>
    public Guid OrganizationId { get; set; } = default!;

    /// <summary>
    /// Идентификатор единицы складского учета
    /// </summary>
    public Guid SkuId { get; set; } = default!;


    public virtual Sku? Sku { get; set; }
}
