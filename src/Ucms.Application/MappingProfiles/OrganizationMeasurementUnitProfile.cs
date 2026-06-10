namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class OrganizationMeasurementUnitProfile : Profile
{
    public OrganizationMeasurementUnitProfile()
    {
        CreateMap<OrganizationMeasurementUnit, OrganizationMeasurementUnitModel>();
    }
}
