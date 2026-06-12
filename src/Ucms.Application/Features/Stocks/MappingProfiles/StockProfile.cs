namespace Ucms.Application.Features.Stocks;

using AutoMapper;
using Ucms.Domain.Entities;

public class StockProfile : Profile
{
    public StockProfile()
    {
        CreateMap<Stock, StockModel>();
    }
}
