namespace Ucms.Application.MappingProfiles;

using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

public class SupplierProfile : Profile
{
    public SupplierProfile()
    {
        CreateMap<Supplier, SupplierModel>();
    }
}
