namespace Ucms.Stock.Domain.Models.Enums;

using System.ComponentModel.DataAnnotations;
using Ucms.Manuals.StaticManuals;

/// <summary>
/// Категория склада
/// </summary>
public enum StockCategory
{
    /// <summary>
    /// По Умолчанию
    /// </summary>
    [Display(Name = "StockCategory_Default")]
    [ManualValue("Podstansiyalarda", "На подстанциях", "In substation", "Podstansiyalarda")]
    Default = 0,

    /// <summary>
    /// Центральный
    /// </summary>
    [Display(Name = "StockCategory_Central")]
    [ManualValue("Asosiy omborda", "На основном складе", "In the main warehouse", "Asosiy omborda")]
    Central = 10
}
