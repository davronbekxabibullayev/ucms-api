namespace Ucms.Application.DTOs.Models;

public record ManufacturerModel(Guid Id,
                                string Name,
                                string NameRu,
                                string? NameEn,
                                string? NameKa,
                                string? Code);
