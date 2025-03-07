namespace Ucms.Stock.Contracts.Models;

using Ucms.Stock.Domain.Models.Enums;

public record SkuModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameRu { get; set; } = default!;
    public string? NameEn { get; set; }
    public string? NameKa { get; set; }
    public string SerialNumber { get; set; } = default!;
    public ManufacturerModel Manufacturer { get; set; } = default!;
    public MeasurementUnitModel MeasurementUnit { get; set; } = default!;
    public SupplierModel Supplier { get; set; } = default!;
    public Guid ProductId { get; set; }
    public Guid? ManufacturerId { get; set; }
    public Guid MeasurementUnitId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal StockSkuAmount { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public ProductType ProductType { get; set; }
    public SkuStatus Status { get; set; }
}
