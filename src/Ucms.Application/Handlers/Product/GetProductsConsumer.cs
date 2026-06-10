namespace Ucms.Application.Handlers.Product;

using AutoMapper;
using Ucms.Domain.Enums;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetProductsMessage(string? Query, List<ProductType>? Type, PagedRequest Request) : IRequest<PagedResult<ProductModel>>;

public class GetProductsConsumer : RequestHandler<GetProductsMessage, PagedResult<ProductModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetProductsConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<ProductModel>> Handle(GetProductsMessage message,
        CancellationToken cancellationToken)
    {
        return await GetProducts(message.Query, message.Type, message.Request);
    }

    private async Task<PagedResult<ProductModel>> GetProducts(string? query, List<ProductType>? type, PagedRequest? paging)
    {
        var productsQuery = _dbContext.Products
            .OrderBy(x => x.Name)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            query = query.ToLowerInvariant().Trim();
            productsQuery = productsQuery.Where(x =>
                x.Name.ToLower().Contains(query)
                || x.NameRu.ToLower().Contains(query)
                || x.NameKa!.ToLower().Contains(query)
                || x.NameEn!.ToLower().Contains(query)
                || x.Code!.ToLower().Contains(query));
        }

        if (type != null)
        {
            productsQuery = productsQuery.Where(w => type.Contains(w.Type));
        }

        return await productsQuery.ToPagedResultAsync<Product, ProductModel>(paging, _mapper);
    }
}
