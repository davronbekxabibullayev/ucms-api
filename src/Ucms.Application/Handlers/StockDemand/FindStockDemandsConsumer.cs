namespace Ucms.Application.Handlers.StockDemand;

using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindStockDemandsMessage(string Query) : IRequest<List<StockDemandModel>>;

public class FindStockDemandsConsumer : RequestHandler<FindStockDemandsMessage, List<StockDemandModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindStockDemandsConsumer(IAppDbContext dbContext, IMapper mapper)
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
