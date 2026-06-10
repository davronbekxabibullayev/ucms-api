namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class StockProfile : Profile
{
    public StockProfile()
    {
        CreateMap<Stock, StockModel>();
    }
}
