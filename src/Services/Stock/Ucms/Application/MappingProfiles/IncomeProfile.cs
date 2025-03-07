namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class IncomeProfile : Profile
{
    public IncomeProfile()
    {
        CreateMap<Income, IncomeModel>()
            .ForMember(dest => dest.OutcomeStockId, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.OutcomeStockId : Guid.Empty))
            .ForMember(dest => dest.OutcomeStock, src => src.MapFrom(m => m.IncomeOutcome != null ? m.IncomeOutcome.OutcomeStock : null));
    }
}
