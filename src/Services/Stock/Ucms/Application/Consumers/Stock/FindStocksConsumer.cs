namespace Ucms.Stock.Api.Application.Consumers.Stock;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record FindStocksMessage(string Query) : IRequest<List<StockModel>>;

public class FindStocksConsumer : RequestHandler<FindStocksMessage, List<StockModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public FindStocksConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<List<StockModel>> Handle(FindStocksMessage message,
        CancellationToken cancellationToken)
    {
        var queryString = message.Query.ToLower();

        var stocks = _dbContext.Stocks
            .Where(a => a.Name.ToLower().Contains(queryString) ||
                        a.NameEn!.ToLower().Contains(queryString) ||
                        a.NameRu.ToLower().Contains(queryString) ||
                        a.NameKa!.ToLower().Contains(queryString) ||
                        a.Code.Contains(queryString));
        if (!_workContext.IsAdmin)
            stocks = stocks.Where(a => a.OrganizationId == _workContext.TenantId);

        return await stocks.OrderBy(a => a.Name)
            .ProjectTo<StockModel>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
