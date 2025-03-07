namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class StockDemandProfile : Profile
{
    public StockDemandProfile()
    {
        CreateMap<StockDemand, StockDemandModel>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.StockDemandItems));

        CreateMap<StockDemand, ReceivedDemandModel>()
            .ForMember(dest => dest.StockName, opt => opt.MapFrom(src => src.Sender!.Name))
            .ForMember(dest => dest.StockNameRu, opt => opt.MapFrom(src => src.Sender!.NameRu))
            .ForMember(dest => dest.StockNameEn, opt => opt.MapFrom(src => src.Sender!.NameEn))
            .ForMember(dest => dest.StockNameKa, opt => opt.MapFrom(src => src.Sender!.NameKa))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.StockDemandItems));

        CreateMap<StockDemand, RequestedDemandModel>()
            .ForMember(dest => dest.StockName, opt => opt.MapFrom(src => src.Sender!.Name))
            .ForMember(dest => dest.StockNameRu, opt => opt.MapFrom(src => src.Sender!.NameRu))
            .ForMember(dest => dest.StockNameEn, opt => opt.MapFrom(src => src.Sender!.NameEn))
            .ForMember(dest => dest.StockNameKa, opt => opt.MapFrom(src => src.Sender!.NameKa));
    }
}
