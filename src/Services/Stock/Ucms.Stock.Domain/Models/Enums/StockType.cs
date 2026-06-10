namespace Ucms.Stock.Domain.Models.Enums;

using System.ComponentModel.DataAnnotations;
using Ucms.Manuals.StaticManuals;

/// <summary>
/// Тип склада
/// </summary>
public enum StockType
{
    /// <summary>
    /// Не определен
    /// </summary>
    [Display(Name = "StockType_NotApplicable")]
    [ManualValue("Aniqlanmagan", "Не определен", "Not defined", "Aniqlanmagan")]
    NotApplicable = 0,

    /// <summary>
    /// Здание
    /// </summary>
    [Display(Name = "StockType_Building")]
    [ManualValue("Binoda", "На здание", "In building", "Binoda")]
    Building = 10,

    /// <summary>
    /// Помещение
    /// </summary>
    [Display(Name = "StockType_Premises")]
    [ManualValue("Xonada", "На помещение", "In room", "Xonada")]
    Premises = 20,

    /// <summary>
    /// Машина
    /// </summary>
    [Display(Name = "StockType_Car")]
    [ManualValue("Mashinada", "В машине", "In car", "Mashinada")]
    Car = 30,

    /// <summary>
    /// Чемодан
    /// </summary>
    [Display(Name = "StockType_Case")]
    [ManualValue("Sumkada", "В сумке", "In case", "Sumkada")]
    Case = 40
}
