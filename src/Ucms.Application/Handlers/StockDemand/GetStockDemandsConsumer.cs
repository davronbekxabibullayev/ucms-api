namespace Ucms.Application.Handlers.StockDemand;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetStockDemandsMessage : IRequest<List<StockDemandModel>>;
public class GetStockDemandsConsumer : RequestHandler<GetStockDemandsMessage, List<StockDemandModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetStockDemandsConsumer(IUcmsDbContext dbContext, IMapper mapper)
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
