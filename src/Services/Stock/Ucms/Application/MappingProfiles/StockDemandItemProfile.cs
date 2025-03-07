namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class StockDemandItemProfile : Profile
{
    public StockDemandItemProfile()
    {
        CreateMap<StockDemandItem, StockDemandItemModel>();
    }
}
