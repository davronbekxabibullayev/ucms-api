namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateStockDemandBroadcastStatusMessage(Guid Id, Guid OutcomeId, StockDemandBroadcastStatus Status) : IRequest<Guid>;

public class UpdateStockDemandBroadcastStatusConsumer : RequestHandler<UpdateStockDemandBroadcastStatusMessage, Guid>
{
    private readonly IStockDbContext _dbContext;

    public UpdateStockDemandBroadcastStatusConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<Guid> Handle(UpdateStockDemandBroadcastStatusMessage message, CancellationToken cancellationToken)
    {
        var stockDemand = await GetStockDemandOrThrowAsync(message, cancellationToken);

        ValidateIsStatus(stockDemand);

        stockDemand.BroadcastStatus = message.Status;
        stockDemand.OutcomeId = message.OutcomeId;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return stockDemand.Id;
    }

    private static void ValidateIsStatus(StockDemand stockDemand)
    {
        if (stockDemand.BroadcastStatus is StockDemandBroadcastStatus.Approved or StockDemandBroadcastStatus.Cancelled)
            throw new AppException($"Статус потребности не может быть изменен");
    }

    private async Task<StockDemand> GetStockDemandOrThrowAsync(UpdateStockDemandBroadcastStatusMessage message, CancellationToken cancellationToken)
    {
        return await _dbContext.StockDemands
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"StockDemand with ID: {message.Id}, not found!");
    }
}
