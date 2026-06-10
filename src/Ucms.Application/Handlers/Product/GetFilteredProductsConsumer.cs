namespace Ucms.Application.Handlers.Product;

using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetFilteredProductsMessage(PagedRequest Filter) : IRequest<PagedResult<ProductModel>>;

public class GetFilteredProductsConsumer : RequestHandler<GetFilteredProductsMessage, PagedResult<ProductModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFilteredProductsConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<ProductModel>> Handle(GetFilteredProductsMessage message,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .OrderBy(c => c.Name)
            .ToPagedResultAsync<Product, ProductModel>(message.Filter, _mapper, cancellationToken);
    }
}
