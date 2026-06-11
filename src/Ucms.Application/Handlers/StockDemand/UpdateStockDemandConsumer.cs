namespace Ucms.Application.Handlers.StockDemand;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Application.Services;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record UpdateStockDemandMessage(
    Guid Id,
    string Name,
    string? Note,
    DateTimeOffset DemandDate,
    StockDemandStatus DemandStatus,
    Guid SenderId,
    Guid RecipientId,
    IEnumerable<StockDemandItemModel> Items) : IRequest<Guid>;

public class UpdateStockDemandConsumer(
    IUcmsDbContext dbContext,
    IWorkContext workContext,
    IOrganizationService organizationService) : RequestHandler<UpdateStockDemandMessage, Guid>
{
    protected override async Task<Guid> Handle(UpdateStockDemandMessage message, CancellationToken cancellationToken)
    {
        var stockDemand = await GetStockDemandOrThrow(message, cancellationToken);

        stockDemand.StockDemandItems.Clear();

        await MapToEntity(message, stockDemand);

        NewStockDemandItems(message, stockDemand);

        await dbContext.SaveChangesAsync(cancellationToken);

        return stockDemand.Id;
    }

    private async Task<StockDemand> GetStockDemandOrThrow(UpdateStockDemandMessage message, CancellationToken cancellationToken)
    {
        var stockDemand = await dbContext.StockDemands
            .AsTracking()
            .Include(i => i.StockDemandItems)
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken);

        if (stockDemand == null || stockDemand.DemandStatus != StockDemandStatus.Draft)
            throw new NotFoundException($"StockDemand with ID: {message.Id}, not found!");

        return stockDemand;
    }

    private static void NewStockDemandItems(UpdateStockDemandMessage message, StockDemand stockDemand)
    {
        foreach (var item in message.Items)
            stockDemand.StockDemandItems.Add(NewStockDemandItem(item));
    }

    private static StockDemandItem NewStockDemandItem(StockDemandItemModel item)
    {
        return new StockDemandItem
        {
            Amount = item.Amount,
            ProductId = item.ProductId,
            MeasurementUnitId = item.MeasurementUnitId,
            NotApproved = item.NotApproved,
            Note = item.Note
        };
    }

    private async Task MapToEntity(UpdateStockDemandMessage message, StockDemand? stockDemand)
    {
        stockDemand.Name = message.Name;
        stockDemand.Note = message.Note;
        stockDemand.DemandStatus = message.DemandStatus;
        stockDemand.DemandDate = message.DemandDate;
        stockDemand.SenderId = message.SenderId;
        stockDemand.RecipientId = message.RecipientId;
        stockDemand.EmployeeId = workContext.EmployeeId;
        stockDemand.EmployeeName = await organizationService.GetEmployeeName(workContext.EmployeeId);
        stockDemand.StockDemandItems = [];
    }
}
