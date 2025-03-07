namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class OutcomeItemProfile : Profile
{
    public OutcomeItemProfile()
    {
        CreateMap<OutcomeItem, OutcomeItemModel>()
            .ForMember(dest => dest.ProductType, act => act.MapFrom(src => src.Sku != null && src.Sku.Product != null ? src.Sku.Product.Type : ProductType.Default));
        CreateMap<OutcomeItem, CreateOutcomeItemModel>();
    }
}
