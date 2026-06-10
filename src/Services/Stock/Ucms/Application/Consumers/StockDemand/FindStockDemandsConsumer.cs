namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record FindStockDemandsMessage(string Query) : IRequest<List<StockDemandModel>>;

public class FindStockDemandsConsumer : RequestHandler<FindStockDemandsMessage, List<StockDemandModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindStockDemandsConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<List<StockDemandModel>> Handle(FindStockDemandsMessage message, CancellationToken cancellationToken)
    {
        var query = message.Query.ToLower();

        var stockDemands = await _dbContext.StockDemands
            .Where(a => a.Name.ToLower().Contains(query) ||
                        a.Note!.ToLower().Contains(query))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<StockDemandModel>>(stockDemands);

        return result;
    }
}
