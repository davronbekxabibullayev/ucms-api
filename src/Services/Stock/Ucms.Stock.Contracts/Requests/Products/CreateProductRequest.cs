namespace Ucms.Stock.Contracts.Requests.Products;

using Ucms.Stock.Domain.Models.Enums;

public record CreateProductRequest
{
    /// <summary>
    /// Наименование на узбекском объязательно для заполнение
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Наименование на русском обязательно для заполненние
    /// </summary>
    public string NameRu { get; init; } = default!;

    /// <summary>
    /// Наименование на английском
    /// </summary>
    public string? NameEn { get; init; }

    /// <summary>
    /// Наименование на каракалпакском
    /// </summary>
    public string? NameKa { get; init; }

    /// <summary>
    /// Код товара
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Международный код
    /// </summary>
    public string? InternationalCode { get; set; }

    /// <summary>
    /// Международный наименование
    /// </summary>
    public string? InternationalName { get; set; }

    /// <summary>
    /// Альтернативное наименование
    /// </summary>
    public string? AlternativeName { get; set; }

    /// <summary>
    /// Тип продукта
    /// </summary>
    public ProductType Type { get; set; }
}
