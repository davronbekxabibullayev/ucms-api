namespace Ucms.Application.Handlers.Stock;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetStockMessage(Guid Id) : IRequest<StockModel>;

public class GetStockConsumer : RequestHandler<GetStockMessage, StockModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetStockConsumer(IAppDbContext dbContext, IMapper mapper)
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
