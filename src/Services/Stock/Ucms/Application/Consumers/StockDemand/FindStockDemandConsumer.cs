namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record FindStockDemandMessage(string Name) : IRequest<StockDemandModel>;

public class FindStockDemandConsumer : RequestHandler<FindStockDemandMessage, StockDemandModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindStockDemandConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<StockDemandModel> Handle(FindStockDemandMessage message, CancellationToken cancellationToken)
    {
        var stockDemand = await _dbContext.StockDemands
            .Include(a => a.StockDemandItems)
            .FirstOrDefaultAsync(f => f.Name == message.Name, cancellationToken)
            ?? throw new NotFoundException($"StockDemand with Name: {message.Name}, not found!");

        return _mapper.Map<StockDemandModel>(stockDemand);
    }
}
