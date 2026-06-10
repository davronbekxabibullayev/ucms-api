namespace Ucms.Application.Handlers.StockDemand;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record UpdateStockDemandStatusMessage(Guid Id, StockDemandStatus Status) : IRequest<Guid>;

public class UpdateStockDemandStatusConsumer : RequestHandler<UpdateStockDemandStatusMessage, Guid>
{
    private readonly IAppDbContext _dbContext;

    public UpdateStockDemandStatusConsumer(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<Guid> Handle(UpdateStockDemandStatusMessage message, CancellationToken cancellationToken)
    {
        var stockDemand = await GetStockDemandOrThrowAsync(message, cancellationToken);

        ValidateIsApproved(stockDemand);

        stockDemand.DemandStatus = message.Status;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return stockDemand.Id;
    }

    private static void ValidateIsApproved(StockDemand stockDemand)
    {
        if (stockDemand.DemandStatus is StockDemandStatus.Approved or StockDemandStatus.Cancelled)
            throw new AppException($"Статус потребности не может быть изменен");
    }

    private async Task<StockDemand> GetStockDemandOrThrowAsync(UpdateStockDemandStatusMessage message, CancellationToken cancellationToken)
    {
        return await _dbContext.StockDemands
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"StockDemand with ID: {message.Id}, not found!");
    }
}
