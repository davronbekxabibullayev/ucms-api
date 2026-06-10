namespace Ucms.Application.Handlers.StockDemand;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record UpdateStockDemandBroadcastStatusMessage(Guid Id, Guid OutcomeId, StockDemandBroadcastStatus Status) : IRequest<Guid>;

public class UpdateStockDemandBroadcastStatusConsumer : RequestHandler<UpdateStockDemandBroadcastStatusMessage, Guid>
{
    private readonly IAppDbContext _dbContext;

    public UpdateStockDemandBroadcastStatusConsumer(IAppDbContext dbContext)
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
