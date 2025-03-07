namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class MeasurementUnitProfile : Profile
{
    public MeasurementUnitProfile()
    {
        CreateMap<MeasurementUnit, MeasurementUnitModel>();
    }
}
