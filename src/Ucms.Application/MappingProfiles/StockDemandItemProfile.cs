namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class StockDemandItemProfile : Profile
{
    public StockDemandItemProfile()
    {
        CreateMap<StockDemandItem, StockDemandItemModel>();
    }
}
