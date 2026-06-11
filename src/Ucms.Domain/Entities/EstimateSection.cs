namespace Ucms.Domain.Entities;

using Ucms.Domain.Common;

/// <summary>
/// Smeta bo'limi (e.g. "Полы", "Потолки", "Стены")
/// </summary>
public class EstimateSection : Entity
{
    /// <summary>
    /// Loyiha ID
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Bo'lim nomi
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Tartib raqami
    /// </summary>
    public int Order { get; set; }

    public virtual Project? Project { get; set; }
    public virtual ICollection<EstimateItem> EstimateItems { get; set; } = [];
}
