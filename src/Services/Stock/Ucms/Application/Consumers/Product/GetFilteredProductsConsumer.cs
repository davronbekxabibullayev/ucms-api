namespace Ucms.Stock.Api.Application.Consumers.Product;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Models;
using Ucms.Common.Paging;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredProductsMessage(FilteringRequest Filter) : IRequest<TableDataResult<List<ProductModel>>>;

public class
    GetFilteredProductsConsumer : RequestHandler<GetFilteredProductsMessage, TableDataResult<List<ProductModel>>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetFilteredProductsConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<TableDataResult<List<ProductModel>>> Handle(GetFilteredProductsMessage message,
        CancellationToken cancellationToken)
    {
        var queryResult = await _dbContext.Products
            .Where(w => w.EmergencyType == _workContext.EmergencyType)
            .OrderBy(c => c.Name)
            .AsFilterable(message.Filter, out var total)
            .ToListAsync(cancellationToken);

        var products = _mapper.Map<List<ProductModel>>(queryResult);

        var result = new TableDataResult<List<ProductModel>>
        {
            Data = products,
            Total = total
        };

        return result;
    }
}
