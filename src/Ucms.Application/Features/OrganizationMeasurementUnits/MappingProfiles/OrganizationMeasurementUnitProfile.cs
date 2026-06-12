namespace Ucms.Application.Features.OrganizationMeasurementUnits;

using AutoMapper;
using Ucms.Domain.Entities;

public class OrganizationMeasurementUnitProfile : Profile
{
    public OrganizationMeasurementUnitProfile()
    {
        CreateMap<OrganizationMeasurementUnit, OrganizationMeasurementUnitModel>();
    }
}
