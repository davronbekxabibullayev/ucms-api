namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateStockDemandStatusMessage(Guid Id, StockDemandStatus Status) : IRequest<Guid>;

public class UpdateStockDemandStatusConsumer : RequestHandler<UpdateStockDemandStatusMessage, Guid>
{
    private readonly IStockDbContext _dbContext;

    public UpdateStockDemandStatusConsumer(IStockDbContext dbContext)
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
