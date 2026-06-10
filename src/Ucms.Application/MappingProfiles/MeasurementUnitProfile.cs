namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class MeasurementUnitProfile : Profile
{
    public MeasurementUnitProfile()
    {
        CreateMap<MeasurementUnit, MeasurementUnitModel>();
    }
}
