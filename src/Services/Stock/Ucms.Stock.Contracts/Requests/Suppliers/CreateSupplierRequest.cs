namespace Ucms.Stock.Contracts.Requests.Suppliers;

public record CreateSupplierRequest
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
}
