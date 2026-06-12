namespace Ucms.Application.Features.Incomes;

using AutoMapper;
using Ucms.Domain.Entities;

public class IncomeItemProfile : Profile
{
    public IncomeItemProfile()
    {
        CreateMap<IncomeItem, IncomeItemModel>();
    }
}
