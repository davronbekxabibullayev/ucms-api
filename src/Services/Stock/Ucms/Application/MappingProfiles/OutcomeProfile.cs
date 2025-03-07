namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class OutcomeProfile : Profile
{
    public OutcomeProfile()
    {
        CreateMap<Outcome, OutcomeModel>()
            .ForMember(dest => dest.IncomeStockId, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.IncomeStockId : Guid.Empty))
            .ForMember(dest => dest.IncomeStock, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.IncomeStock : null));
    }
}
