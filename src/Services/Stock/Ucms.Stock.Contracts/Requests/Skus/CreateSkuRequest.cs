namespace Ucms.Stock.Contracts.Requests.Skus;

using Ucms.Stock.Domain.Models.Enums;

public record CreateSkuRequest
{
    public string Name { get; init; } = default!;
    public string NameRu { get; init; } = default!;
    public string? NameEn { get; init; }
    public string? NameKa { get; init; }
    public string SerialNumber { get; init; } = default!;
    public Guid ProductId { get; init; }
    public Guid? ManufacturerId { get; set; }
    public Guid MeasurementUnitId { get; init; }
    public Guid? SupplierId { get; init; }
    public decimal Price { get; init; }
    public decimal Amount { get; init; }
    public DateTimeOffset ExpirationDate { get; set; }
    public SkuStatus Status { get; set; } = SkuStatus.Default;
}
