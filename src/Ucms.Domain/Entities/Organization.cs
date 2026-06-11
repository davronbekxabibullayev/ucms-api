namespace Ucms.Domain.Entities;

using Ucms.Domain.Common;

/// <summary>
/// Tashkilot (bizning kompaniya yoki ish beruvchi)
/// </summary>
public class Organization : AuditableEntity, IDeletable
{
    /// <summary>
    /// Tashkilot nomi
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// INN (soliq raqami)
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Manzil
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Telefon raqami
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Elektron pochta
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// O'chirilgan yoki yo'q
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = [];
    public virtual ICollection<Brigade> Brigades { get; set; } = [];
}
