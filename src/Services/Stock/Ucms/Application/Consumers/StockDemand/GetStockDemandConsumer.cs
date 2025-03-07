namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetStockDemandMessage(Guid Id) : IRequest<StockDemandModel>;
public class GetStockDemandConsumer : RequestHandler<GetStockDemandMessage, StockDemandModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetStockDemandConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<StockDemandModel> Handle(GetStockDemandMessage message, CancellationToken cancellationToken)
    {
        var stockDemand = await _dbContext.StockDemands
            .Include(i => i.StockDemandItems)
            .ThenInclude(th => th.Product)
            .Include(i => i.StockDemandItems)
            .ThenInclude(th => th.MeasurementUnit)
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"StockDemand with ID: {message.Id}, not found!");

        return _mapper.Map<StockDemandModel>(stockDemand);
    }
}
