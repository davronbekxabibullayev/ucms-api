namespace Ucms.Stock.Api.Application.Consumers.Stock;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetCentralStocksMessage(Guid OrganizationId) : IRequest<List<StockModel>>;

public class GetCentralStocksConsumer : RequestHandler<GetCentralStocksMessage, List<StockModel>>
{
    private readonly IMapper _mapper;
    private readonly IStockDbContext _dbContext;

    public GetCentralStocksConsumer(IMapper mapper, IStockDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    protected override async Task<List<StockModel>> Handle(GetCentralStocksMessage message, CancellationToken cancellationToken)
    {
        var stocks = _dbContext.Stocks
            .Where(a => a.StockCategory == Common.Enums.StockCategory.Central);

        stocks = stocks.Where(w => w.OrganizationId == message.OrganizationId);

        stocks = stocks.OrderBy(a => a.Name);

        return _mapper.Map<List<StockModel>>(await stocks.ToListAsync(cancellationToken));
    }
}
