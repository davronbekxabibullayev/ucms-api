namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Services;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record CreateStockDemandMessage(
    string Name,
    string? Note,
    DateTimeOffset DemandDate,
    StockDemandStatus DemandStatus,
    Guid SenderId,
    Guid RecipientId,
    IEnumerable<StockDemandItemModel> Items) : IRequest<Guid>;

public class CreateStockDemandConsumer(
    IStockDbContext dbContext,
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
