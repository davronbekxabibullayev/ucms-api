namespace Ucms.Domain.Entities;

using Ucms.Domain.Common;

/// <summary>
/// Smeta qatori — bir ish turi (e.g. "Стяжка пола", "Штукатурка стен")
/// </summary>
public class EstimateItem : Entity
{
    /// <summary>
    /// Smeta bo'limi ID
    /// </summary>
    public Guid SectionId { get; set; }

    /// <summary>
    /// Ish nomi
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// O'lchov birligi (m², m³, dona, m.p.)
    /// </summary>
    public string Unit { get; set; } = default!;

    /// <summary>
    /// Smeta bo'yicha umumiy hajm (zakazchik bilan kelishilgan)
    /// </summary>
    public decimal Volume { get; set; }

    /// <summary>
    /// Birlik narhi — zakazchik bilan (so'm)
    /// </summary>
    public decimal ClientUnitPrice { get; set; }

    /// <summary>
    /// Birlik narhi — brigada uchun (so'm), odatiy qiymat
    /// </summary>
    public decimal BrigadeUnitPrice { get; set; }

    /// <summary>
    /// Tartib raqami
    /// </summary>
    public int Order { get; set; }

    public virtual EstimateSection? Section { get; set; }
    public virtual ICollection<WorkLog> WorkLogs { get; set; } = [];
    public virtual ICollection<ClientActItem> ClientActItems { get; set; } = [];
}
