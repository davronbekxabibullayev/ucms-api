namespace Ucms.Stock.Api.Application.MappingProfiles;

using AutoMapper;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public class OrganizationMeasurementUnitProfile : Profile
{
    public OrganizationMeasurementUnitProfile()
    {
        CreateMap<OrganizationMeasurementUnit, OrganizationMeasurementUnitModel>();
    }
}
