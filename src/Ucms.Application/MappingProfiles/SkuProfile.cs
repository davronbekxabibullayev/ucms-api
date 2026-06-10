namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Domain.Enums;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class SkuProfile : Profile
{
    public SkuProfile()
    {
        CreateMap<Sku, SkuModel>()
            .ForMember(dest => dest.ProductType, act => act.MapFrom(src => src.Product != null ? src.Product.Type : ProductType.Default));
    }
}
