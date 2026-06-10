namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class IncomeItemProfile : Profile
{
    public IncomeItemProfile()
    {
        CreateMap<IncomeItem, IncomeItemModel>();
    }
}
