namespace Ucms.Application.Handlers.Stock;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;
using Ucms.Application.Abstractions.Mediator;

public record GetCentralStocksMessage(Guid OrganizationId) : IRequest<List<StockModel>>;

public class GetCentralStocksConsumer : RequestHandler<GetCentralStocksMessage, List<StockModel>>
{
    private readonly IMapper _mapper;
    private readonly IUcmsDbContext _dbContext;

    public GetCentralStocksConsumer(IMapper mapper, IUcmsDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    protected override async Task<List<StockModel>> Handle(GetCentralStocksMessage message, CancellationToken cancellationToken)
    {
        var stocks = _dbContext.Stocks
            .Where(a => a.StockCategory == StockCategory.Central);

        stocks = stocks.Where(w => w.OrganizationId == message.OrganizationId);

        stocks = stocks.OrderBy(a => a.Name);

        return _mapper.Map<List<StockModel>>(await stocks.ToListAsync(cancellationToken));
    }
}
