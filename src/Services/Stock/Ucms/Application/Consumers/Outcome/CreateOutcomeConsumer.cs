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

public record CreateOutcomeMessage(
    string Name,
    string? Note,
    OutcomeType OutcomeType,
    OutcomeStatus OutcomeStatus,
    PaymentType PaymentType,
    DateTimeOffset OutcomeDate,
    Guid StockId,
    Guid? IncomeStockId,
    Guid? ExecutionId,
    IEnumerable<CreateOutcomeItemModel> OutcomeItems) : IRequest<Guid>;

public class CreateOutcomeConsumer(
    IStockDbContext dbContext,
    IOutcomeService outcomeService,
    IWorkContext workContext,
    IOrganizationService organizationService) : RequestHandler<CreateOutcomeMessage, Guid>
{
    protected override async Task<Guid> Handle(CreateOutcomeMessage message, CancellationToken cancellationToken)
    {
        await outcomeService.ValidateOutcomeItems(message.OutcomeItems, message.StockId, cancellationToken);

        var strategy = dbContext.CreateExecutionStrategy();
        var outcomeId = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var outcome = await GetOutcome(message);

                dbContext.Outcomes.Add(outcome);
                await dbContext.SaveChangesAsync(cancellationToken);

                if (outcome.OutcomeStatus == OutcomeStatus.Approved &&
                    (outcome.OutcomeType == OutcomeType.WriteOff ||
                    outcome.OutcomeType == OutcomeType.Usage ||
                    (outcome.OutcomeType == OutcomeType.Return && message.IncomeStockId == null)))
                {
                    await outcomeService.UpdateBalanceAsync(outcome, cancellationToken);
                }

                if (outcome.OutcomeType is OutcomeType.Broadcast or OutcomeType.Return && message.IncomeStockId != null)
                {
                    var incomeOutcome = outcomeService.CreateIncomeOutcome(outcome, (Guid)message.IncomeStockId);

                    if (outcome.OutcomeStatus == OutcomeStatus.Approved)
                    {
                        incomeOutcome.Income = outcomeService.CreateIncome(outcome, (Guid)message.IncomeStockId);

                        var organizationSkus = outcomeService.CreateOrganizationSkus(outcome, (Guid)message.IncomeStockId);
                        dbContext.OrganizationSkus.AddRange(organizationSkus);
                    }

                    dbContext.IncomeOutcomes.Add(incomeOutcome);
                    await dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync(cancellationToken);
                return outcome.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new AppException(ex, "Ошибка при создание расхода!");
            }
        });

        return outcomeId;
    }

    private async Task<Outcome> GetOutcome(CreateOutcomeMessage message)
    {
        return new Outcome
        {
            Name = message.Name,
            Note = message.Note,
            OutcomeType = message.OutcomeType,
            OutcomeStatus = message.OutcomeStatus,
            OutcomeTransferStatus = GetTransferStatus(message),
            PaymentType = message.PaymentType,
            OutcomeDate = message.OutcomeDate,
            StockId = message.StockId,
            EmployeeId = workContext.EmployeeId,
            ExecutionId = message.ExecutionId,
            EmployeeName = await organizationService.GetEmployeeName(workContext.EmployeeId),
            OutcomeItems = message.OutcomeItems.Select(s => new OutcomeItem
            {
                SkuId = s.SkuId,
                Amount = s.Amount,
                MeasurementUnitId = s.MeasurementUnitId
            }).ToArray()
        };
    }

    private static OutcomeTransferStatus? GetTransferStatus(CreateOutcomeMessage message)
    {
        return (message.OutcomeType == OutcomeType.Broadcast || message.OutcomeType == OutcomeType.Return)
                && message.IncomeStockId != null && message.OutcomeStatus == OutcomeStatus.Approved
                ? OutcomeTransferStatus.Sent : null;
    }
}
