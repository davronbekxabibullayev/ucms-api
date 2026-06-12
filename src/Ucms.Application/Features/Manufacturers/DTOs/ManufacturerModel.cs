namespace Ucms.Application.Features.Manufacturers;

public record ManufacturerModel(Guid Id,
                                string Name,
                                string NameRu,
                                string? NameEn,
                                string? NameKa,
                                string? Code);
