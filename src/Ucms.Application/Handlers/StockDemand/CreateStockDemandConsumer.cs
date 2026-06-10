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

public record CreateStockDemandMessage(
    string Name,
    string? Note,
    DateTimeOffset DemandDate,
    StockDemandStatus DemandStatus,
    Guid SenderId,
    Guid RecipientId,
    IEnumerable<StockDemandItemModel> Items) : IRequest<Guid>;

public class CreateStockDemandConsumer(
    IAppDbContext dbContext,
    IWorkContext workContext,
    IOrganizationService organizationService) : RequestHandler<CreateStockDemandMessage, Guid>
{
    protected override async Task<Guid> Handle(CreateStockDemandMessage message, CancellationToken cancellationToken)
    {
        var exist = await dbContext.StockDemands
            .AnyAsync(f => f.Name == message.Name, cancellationToken);

        if (exist)
            throw new AlreadyExistException(nameof(StockDemand), message.Name);

        var stockDemand = new StockDemand
        {
            Name = message.Name,
            Note = message.Note,
            DemandStatus = message.DemandStatus,
            DemandDate = message.DemandDate,
            SenderId = message.SenderId,
            RecipientId = message.RecipientId,
            EmployeeId = workContext.EmployeeId,
            EmployeeName = await organizationService.GetEmployeeName(workContext.EmployeeId)
        };

        foreach (var item in message.Items)
            stockDemand.StockDemandItems.Add(new StockDemandItem
            {
                Amount = item.Amount,
                ProductId = item.ProductId,
                MeasurementUnitId = item.MeasurementUnitId,
                NotApproved = item.NotApproved,
                Note = item.Note
            });

        dbContext.StockDemands.Add(stockDemand);
        await dbContext.SaveChangesAsync(cancellationToken);

        return stockDemand.Id;
    }
}
