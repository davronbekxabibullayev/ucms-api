namespace Ucms.Domain.Entities;

using Ucms.Domain.Common;

/// <summary>
/// Smeta bo'limi (e.g. "Pollar", "Devorlar", "Shiftlar")
/// </summary>
public class EstimateSection : Entity
{
    /// <summary>
    /// Smeta ID (Estimate → Sections)
    /// </summary>
    public Guid EstimateId { get; set; }

    /// <summary>
    /// Bo'lim nomi
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Tartib raqami
    /// </summary>
    public int Order { get; set; }

    public virtual Estimate? Estimate { get; set; }
    public virtual ICollection<EstimateItem> EstimateItems { get; set; } = [];
}
