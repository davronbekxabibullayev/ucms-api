namespace Ucms.Stock.Api.Application.Consumers.Product;

using AutoMapper;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Common.Paging;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetProductsMessage(string? Query, List<ProductType>? Type, PagingRequest Request) : IRequest<PagedList<ProductModel>>;

public class GetProductsConsumer : RequestHandler<GetProductsMessage, PagedList<ProductModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetProductsConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<PagedList<ProductModel>> Handle(GetProductsMessage message,
        CancellationToken cancellationToken)
    {
        return await GetProducts(message.Query, message.Type, message.Request);
    }

    private async Task<PagedList<ProductModel>> GetProducts(string? query, List<ProductType>? type, PagingRequest? paging)
    {
        var productsQuery = _dbContext.Products
            .Where(w => w.EmergencyType == _workContext.EmergencyType)
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

        return await productsQuery.ToPagedListAsync<Product, ProductModel>(paging, _mapper);
    }
}
