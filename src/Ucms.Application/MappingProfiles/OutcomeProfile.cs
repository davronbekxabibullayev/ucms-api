namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class OutcomeProfile : Profile
{
    public OutcomeProfile()
    {
        CreateMap<Outcome, OutcomeModel>()
            .ForMember(dest => dest.IncomeStockId, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.IncomeStockId : Guid.Empty))
            .ForMember(dest => dest.IncomeStock, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.IncomeStock : null));
    }
}
