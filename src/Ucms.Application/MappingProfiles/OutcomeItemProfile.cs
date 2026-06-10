namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Domain.Enums;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class OutcomeItemProfile : Profile
{
    public OutcomeItemProfile()
    {
        CreateMap<OutcomeItem, OutcomeItemModel>()
            .ForMember(dest => dest.ProductType, act => act.MapFrom(src => src.Sku != null && src.Sku.Product != null ? src.Sku.Product.Type : ProductType.Default));
        CreateMap<OutcomeItem, CreateOutcomeItemModel>();
    }
}
