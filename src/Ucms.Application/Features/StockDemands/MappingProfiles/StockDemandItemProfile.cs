namespace Ucms.Application.Features.StockDemands;

using AutoMapper;
using Ucms.Domain.Entities;

public class StockDemandItemProfile : Profile
{
    public StockDemandItemProfile()
    {
        CreateMap<StockDemandItem, StockDemandItemModel>();
    }
}
