namespace Ucms.Application.DTOs.Models;

public record SupplierModel(Guid Id,
                           string Name,
                           string NameRu,
                           string? NameEn,
                           string? NameKa,
                           string? Code);
