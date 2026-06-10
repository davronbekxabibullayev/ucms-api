namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Contracts.Models;

public class StockProfile : Profile
{
    public StockProfile()
    {
        CreateMap<Domain.Models.Stock, StockModel>();
    }
}
