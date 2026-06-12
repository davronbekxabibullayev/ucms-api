namespace Ucms.Application.Features.StockSkus;

using AutoMapper;
using Ucms.Domain.Entities;

public class StockSkuProfile : Profile
{
    public StockSkuProfile()
    {
        CreateMap<StockSku, StockSkuModel>();
    }
}
