namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using System.Threading;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Services;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateOutcomeMessage(
    Guid Id,
    string Name,
    string? Note,
    OutcomeType OutcomeType,
    OutcomeStatus OutcomeStatus,
    PaymentType PaymentType,
    DateTimeOffset OutcomeDate,
    Guid StockId,
    Guid? IncomeStockId,
    Guid? ExecutionId,
    IEnumerable<CreateOutcomeItemModel> OutcomeItems
) : IRequest<Guid>;

public class UpdateOutcomeConsumer(
    IStockDbContext dbContext,
    IOutcomeService outcomeService,
    IWorkContext workContext,
    IOrganizationService organizationService) : RequestHandler<UpdateOutcomeMessage, Guid>
{
    protected override async Task<Guid> Handle(UpdateOutcomeMessage message, CancellationToken cancellationToken)
    {
        await outcomeService.ValidateOutcomeItems(message.OutcomeItems, message.StockId, cancellationToken);

        var outcome = await dbContext.Outcomes
            .Include(i => i.OutcomeItems)
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Outcome with ID: {message.Id}, not found!");

        var strategy = dbContext.CreateExecutionStrategy();
        var outcomeId = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                dbContext.OutcomeItems.RemoveRange(outcome.OutcomeItems);
                await dbContext.SaveChangesAsync(cancellationToken);

                await UpdateOutcome(outcome, message);

                dbContext.Outcomes.Update(outcome);
                await dbContext.SaveChangesAsync(cancellationToken);

                if (outcome.OutcomeStatus == OutcomeStatus.Approved &&
                    (outcome.OutcomeType == OutcomeType.WriteOff || outcome.OutcomeType == OutcomeType.Usage))
                {
                    await outcomeService.UpdateBalanceAsync(outcome, cancellationToken);
                }

                if ((outcome.OutcomeType is OutcomeType.Broadcast or OutcomeType.Return) && message.IncomeStockId != null)
                {
                    var incomeOutcome = await dbContext.IncomeOutcomes.FirstOrDefaultAsync(f => f.OutcomeId == outcome.Id)
                        ?? outcomeService.CreateIncomeOutcome(outcome, (Guid)message.IncomeStockId);

                    if (outcome.OutcomeStatus == OutcomeStatus.Approved)
                    {
                        incomeOutcome.Income = outcomeService.CreateIncome(outcome, (Guid)message.IncomeStockId);

                        var organizationSkus = outcomeService.CreateOrganizationSkus(outcome, (Guid)message.IncomeStockId);
                        dbContext.OrganizationSkus.AddRange(organizationSkus);
                    }

                    dbContext.IncomeOutcomes.Update(incomeOutcome);
                    await dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync(cancellationToken);
                return outcome.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new AppException(ex, "Ошибка при редактировании расхода!");
            }
        });

        return outcomeId;
    }

    public async Task UpdateOutcome(Outcome outcome, UpdateOutcomeMessage message)
    {
        outcome.Name = message.Name;
        outcome.Note = message.Note;
        outcome.OutcomeType = message.OutcomeType;
        outcome.OutcomeStatus = message.OutcomeStatus;
        outcome.OutcomeTransferStatus = GetTransferStatus(message);
        outcome.PaymentType = message.PaymentType;
        outcome.OutcomeDate = message.OutcomeDate;
        outcome.StockId = message.StockId;
        outcome.EmployeeId = workContext.EmployeeId;
        outcome.ExecutionId = message.ExecutionId;
        outcome.EmployeeName = await organizationService.GetEmployeeName(workContext.EmployeeId);
        outcome.OutcomeItems = message.OutcomeItems.Select(s => new OutcomeItem
        {
            SkuId = s.SkuId,
            Amount = s.Amount,
            MeasurementUnitId = s.MeasurementUnitId
        }).ToArray();
    }

    private static OutcomeTransferStatus? GetTransferStatus(UpdateOutcomeMessage message)
    {
        return (message.OutcomeType == OutcomeType.Broadcast || message.OutcomeType == OutcomeType.Return)
                && message.IncomeStockId != null && message.OutcomeStatus == OutcomeStatus.Approved
                ? OutcomeTransferStatus.Sent : null;
    }
}
