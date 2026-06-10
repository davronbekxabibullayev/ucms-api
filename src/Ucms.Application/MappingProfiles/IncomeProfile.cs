namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class IncomeProfile : Profile
{
    public IncomeProfile()
    {
        CreateMap<Income, IncomeModel>()
            .ForMember(dest => dest.OutcomeStockId, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.OutcomeStockId : Guid.Empty))
            .ForMember(dest => dest.OutcomeStock, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.OutcomeStock : null));
    }
}
