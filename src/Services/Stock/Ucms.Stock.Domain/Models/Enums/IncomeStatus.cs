namespace Ucms.Stock.Domain.Models.Enums;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Статус прихода
/// </summary>
public enum IncomeStatus
{
    /// <summary>
    /// Черновик
    /// </summary>
    [Display(Name = "IncomeStatus_Draft")]
    Draft = 0,

    /// <summary>
    /// Подтвержден
    /// </summary>
    [Display(Name = "IncomeStatus_Approved")]
    Approved = 20,

    /// <summary>
    /// Отменен
    /// </summary>
    [Display(Name = "IncomeStatus_Cancelled")]
    Cancelled = 99,
}
