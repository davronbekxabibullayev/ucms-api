namespace Ucms.Domain.Entities;

using Ucms.Domain.Common;

/// <summary>
/// Xodim maoshi yozuvi
/// Запись о зарплате сотрудника
/// </summary>
public class Salary : AuditableEntity, IDeletable
{
    /// <summary>
    /// Tashkilot ID
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Xodim ismi
    /// </summary>
    public string EmployeeName { get; set; } = default!;

    /// <summary>
    /// Lavozim
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Oy (format: "2026-06")
    /// </summary>
    public string Month { get; set; } = default!;

    /// <summary>
    /// Summa
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Izoh
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// O'chirilgan yoki yo'q
    /// </summary>
    public bool IsDeleted { get; set; }
}
