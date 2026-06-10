namespace Ucms.Stock.Api.Application.Consumers.StockSku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredStockSkusMessage(PagingRequest Paging) : IRequest<PagedList<StockSkuModel>>;

public class GetFilteredStockSkusConsumer : RequestHandler<GetFilteredStockSkusMessage, PagedList<StockSkuModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFilteredStockSkusConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedList<StockSkuModel>> Handle(GetFilteredStockSkusMessage message,
        CancellationToken cancellationToken)
    {
        return await _dbContext.StockSkus
            .Include(i => i.Stock)
            .Include(i => i.Sku)
            .OrderBy(a => a.Stock!.Name)
            .ToPagedListAsync<StockSku, StockSkuModel>(message.Paging, _mapper);
    }
}
