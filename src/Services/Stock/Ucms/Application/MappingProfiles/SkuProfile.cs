namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class SkuProfile : Profile
{
    public SkuProfile()
    {
        CreateMap<Sku, SkuModel>()
            .ForMember(dest => dest.ProductType, act => act.MapFrom(src => src.Product != null ? src.Product.Type : ProductType.Default));
    }
}
