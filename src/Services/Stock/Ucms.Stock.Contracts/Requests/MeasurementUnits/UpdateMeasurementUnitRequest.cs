namespace Ucms.Stock.Contracts.Requests.MeasurementUnits;

using Ucms.Stock.Domain.Models.Enums;

public record UpdateMeasurementUnitRequest
{
    public string Name { get; init; } = default!;
    public string NameRu { get; init; } = default!;
    public string? NameEn { get; init; }
    public string? NameKa { get; init; }
    public string Code { get; init; } = default!;
    public decimal Multiplier { get; init; }
    public MeasurementUnitType Type { get; init; }
}
