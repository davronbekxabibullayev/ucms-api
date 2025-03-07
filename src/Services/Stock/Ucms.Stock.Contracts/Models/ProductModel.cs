namespace Ucms.Stock.Contracts.Models;

using Ucms.Stock.Domain.Models.Enums;

public record ProductModel(Guid Id,
                           string Name,
                           string NameRu,
                           string? NameEn,
                           string? NameKa,
                           string? Code,
                           string? InternationalCode,
                           string? InternationalName,
                           string? AlternativeName,
                           ProductType Type);
