namespace Ucms.Stock.Api.Application.Consumers.Product;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Paging;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetOrganizationProductsMessage(Guid? OrganizationId, Guid? StockId, PagingRequest Paging) : IRequest<PagedList<ProductModel>>;

public class GetOrganizationProductsConsumer : RequestHandler<GetOrganizationProductsMessage, PagedList<ProductModel>>
{
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IStockDbContext _dbContext;

    public GetOrganizationProductsConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _workContext = workContext;
    }

    protected override async Task<PagedList<ProductModel>> Handle(GetOrganizationProductsMessage message, CancellationToken cancellationToken)
    {
        var organizationId = message.OrganizationId ?? _workContext.TenantId;
        var query = _dbContext.StockSkus
            .Include(i => i.Sku!.Product)
            .Where(w => w.Stock!.OrganizationId == organizationId);

        if (message.StockId != null)
        {
            query = query.Where(w => w.StockId == message.StockId);
        }

        var productsQuery = query
            .Select(s => s.Sku!.Product!)
            .Distinct()
            .OrderBy(o => o!.Name)
            .AsQueryable();

        return await productsQuery.ToPagedListAsync<Domain.Models.Product, ProductModel>(message.Paging, _mapper);
    }
}
