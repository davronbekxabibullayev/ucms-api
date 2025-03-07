namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetStockDemandsMessage : IRequest<List<StockDemandModel>>;
public class GetStockDemandsConsumer : RequestHandler<GetStockDemandsMessage, List<StockDemandModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetStockDemandsConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<List<StockDemandModel>> Handle(GetStockDemandsMessage message, CancellationToken cancellationToken)
    {
        var stockDemands = await _dbContext.StockDemands
           .OrderBy(a => a.Name)
           .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<StockDemandModel>>(stockDemands);

        return result;
    }
}
