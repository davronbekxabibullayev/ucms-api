namespace Ucms.Stock.Contracts.Models;

using Ucms.Stock.Domain.Models.Enums;

public record MeasurementUnitModel(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string Code,
    decimal Multiplier,
    MeasurementUnitType Type);
