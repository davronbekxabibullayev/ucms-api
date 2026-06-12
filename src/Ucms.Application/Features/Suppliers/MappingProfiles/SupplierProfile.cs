namespace Ucms.Application.Features.Suppliers;

using AutoMapper;
using Ucms.Domain.Entities;

public class SupplierProfile : Profile
{
    public SupplierProfile()
    {
        CreateMap<Supplier, SupplierModel>();
    }
}
