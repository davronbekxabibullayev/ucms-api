namespace Ucms.Stock.Domain.Models.Enums;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Тип продукта
/// </summary>
public enum ProductType
{
    /// <summary>
    /// По умолчанию
    /// </summary>
    [Display(Name = "По умолчанию")]
    Default = 0,

    #region 103

    /// <summary>
    /// Медикамент
    /// </summary>
    [Display(Name = "Медикамент")]
    Medicine = 10,

    /// <summary>
    /// Спец. изделие
    /// </summary>
    [Display(Name = "Спец. изделие")]
    SpecialProduct = 20,

    #endregion


    #region 101

    /// <summary>
    /// Техника
    /// </summary>
    [Display(Name = "Техника")]
    Equipment = 30,

    /// <summary>
    /// Защитные газовые маски
    /// </summary>
    [Display(Name = "Защитные газовые маски")]
    SafetyGasMask = 31,

    /// <summary>
    /// Портативные средства радиосвязи
    /// </summary>
    [Display(Name = "Портативные средства радиосвязи")]
    PortableRadioCommunication = 32,

    /// <summary>
    /// Защитные костюмы
    /// </summary>
    [Display(Name = "Защитные костюмы")]
    ProtectiveSuit = 33,

    /// <summary>
    /// Рукава 
    /// </summary>
    [Display(Name = "Рукава")]
    Sleeve = 34,

    /// <summary>
    /// Всасывающий рукава 
    /// </summary>
    [Display(Name = "Всасывающий рукава")]
    SuctionSleeve = 35,

    /// <summary>
    /// Диэлектрическое  имущество 
    /// </summary>
    [Display(Name = "Диэлектрическое  имущество")]
    Dielectric = 36,

    /// <summary>
    /// Лестница 
    /// </summary>
    [Display(Name = "Лестница")]
    Ladder = 37,

    #endregion

    /// <summary>
    /// Раствор для иньекций
    /// </summary>
    [Display(Name = "Раствор для иньекций")]
    InjectionSolution = 40,

    /// <summary>
    /// Раствор для инфузий
    /// </summary>
    [Display(Name = "Раствор для инфузий")]
    InfusionSolution = 41,

    /// <summary>
    /// Аэрозоль
    /// </summary>
    [Display(Name = "Аэрозоль")]
    Aerosol = 42,

    /// <summary>
    /// Таблетки
    /// </summary>
    [Display(Name = "Таблетки")]
    Pills = 43,

    /// <summary>
    /// Сироп
    /// </summary>
    [Display(Name = "Сироп")]
    Syrup = 44,

    /// <summary>
    /// Мазь
    /// </summary>
    [Display(Name = "Мазь")]
    Ointment = 45,

    /// <summary>
    /// Капли
    /// </summary>
    [Display(Name = "Капли")]
    Drops = 46,

    /// <summary>
    /// Спрей
    /// </summary>
    [Display(Name = "Спрей")]
    Spray = 47,

    /// <summary>
    /// Антисептик
    /// </summary>
    [Display(Name = "Антисептик")]
    Antiseptic = 48,

    /// <summary>
    /// Перевязочный материал
    /// </summary>
    [Display(Name = "Перевязочный материал")]
    DressingMaterial = 49,

    /// <summary>
    /// Одноразовый расходный материал
    /// </summary>
    [Display(Name = "Одноразовый расходный материал")]
    DisposableConsumables = 50,

    /// <summary>
    /// Укладка
    /// </summary>
    [Display(Name = "Укладка")]
    Laying = 51,

    /// <summary>
    /// Диагностика
    /// </summary>
    [Display(Name = "Диагностика")]
    Diagnostics = 52,

    /// <summary>
    /// Дез. средство
    /// </summary>
    [Display(Name = "Дез. средство")]
    DisinfectionAgent = 53,

    /// <summary>
    /// Психотропные вещества
    /// </summary>
    [Display(Name = "Психотропные вещества")]
    PsychotropicSubstances = 54,

    /// <summary>
    /// Наркотические средства
    /// </summary>
    [Display(Name = "Наркотические средства")]
    NarcoticDrugs = 55,

    /// <summary>
    /// Раствор для наружного применения
    /// </summary>
    [Display(Name = "Раствор для наружного применения")]
    SolutionForExternalUse = 56,

    /// <summary>
    /// Порошок
    /// </summary>
    [Display(Name = "Порошок")]
    Powder = 57,

    /// <summary>
    /// Другое
    /// </summary>
    [Display(Name = "Другое")]
    Other = 99,
}
