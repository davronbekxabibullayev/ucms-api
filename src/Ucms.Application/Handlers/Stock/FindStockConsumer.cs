namespace Ucms.Application.Handlers.Stock;

using System.Threading;
using AutoMapper;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record FindStockMessage(string Code) : IRequest<StockModel>;

public class FindStockConsumer : RequestHandler<FindStockMessage, StockModel>
{
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IAppDbContext _dbContext;

    public FindStockConsumer(IAppDbContext dbContext, IMapper mapper, IWorkContext workContext)
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
