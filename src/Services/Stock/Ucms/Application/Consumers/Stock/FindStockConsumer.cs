namespace Ucms.Stock.Api.Application.Consumers.Stock;

using System.Threading;
using AutoMapper;
using Contracts.Models;
using Core.Exceptions;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record FindStockMessage(string Code) : IRequest<StockModel>;

public class FindStockConsumer : RequestHandler<FindStockMessage, StockModel>
{
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IStockDbContext _dbContext;

    public FindStockConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _workContext = workContext;
    }

    protected override async Task<StockModel> Handle(FindStockMessage message, CancellationToken cancellationToken)
    {
        Stock stock;
        if (_workContext.IsAdmin)
            stock = await _dbContext.Stocks.FirstOrDefaultAsync(f => f.Code == message.Code, cancellationToken)
                    ?? throw new NotFoundException($"Stock with code: {message.Code}, not found!");
        else
            stock = await _dbContext.Stocks.FirstOrDefaultAsync(f =>
                        f.Code == message.Code && f.OrganizationId == _workContext.TenantId, cancellationToken)
                    ?? throw new NotFoundException($"Stock with code: {message.Code}, not found!");

        return _mapper.Map<StockModel>(stock);
    }
}
