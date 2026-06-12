namespace Ucms.Application.Features.Products;

using AutoMapper;
using Ucms.Domain.Entities;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductModel>();
    }
}
