namespace Ucms.Application.Features.Manufacturers;

using AutoMapper;
using Ucms.Domain.Entities;

public class ManufacturerProfile : Profile
{
    public ManufacturerProfile()
    {
        CreateMap<Manufacturer, ManufacturerModel>();
    }
}
