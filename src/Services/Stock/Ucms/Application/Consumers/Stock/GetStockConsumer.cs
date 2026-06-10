namespace Ucms.Stock.Api.Application.Consumers.Stock;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetStockMessage(Guid Id) : IRequest<StockModel>;

public class GetStockConsumer : RequestHandler<GetStockMessage, StockModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetStockConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<StockModel> Handle(GetStockMessage message, CancellationToken cancellationToken)
    {
        var stock = await _dbContext.Stocks
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Stock with ID: {message.Id}, not found!");

        return _mapper.Map<StockModel>(stock);
    }
}
