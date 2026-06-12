namespace Ucms.Application.Features.MeasurementUnits;

using AutoMapper;
using Ucms.Domain.Entities;

public class MeasurementUnitProfile : Profile
{
    public MeasurementUnitProfile()
    {
        CreateMap<MeasurementUnit, MeasurementUnitModel>();
    }
}
