namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using System.Threading;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Api.Application.Services;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateOutcomeStatusMessage(
    Guid Id,
    OutcomeStatus Status
) : IRequest<Guid>;

public class UpdateOutcomeStatusConsumer(IStockDbContext dbContext, IOutcomeService outcomeService) : RequestHandler<UpdateOutcomeStatusMessage, Guid>
{
    protected override async Task<Guid> Handle(UpdateOutcomeStatusMessage message, CancellationToken cancellationToken)
    {
        var outcome = await dbContext.Outcomes
            .Include(i => i.OutcomeItems)
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Outcome with ID: {message.Id}, not found!");

        if (outcome.OutcomeStatus == OutcomeStatus.Approved)
            throw new AppException($"Outcome with ID: {message.Id} was approved.");

        await outcomeService.ValidateOutcomeItems(outcome.OutcomeItems, outcome.StockId, cancellationToken);

        var strategy = dbContext.CreateExecutionStrategy();
        var outcomeId = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                outcome.OutcomeStatus = message.Status;
                if (outcome.OutcomeType == OutcomeType.Broadcast && outcome.OutcomeStatus == OutcomeStatus.Approved)
                    outcome.OutcomeTransferStatus = OutcomeTransferStatus.Sent;
                dbContext.Outcomes.Update(outcome);
                await dbContext.SaveChangesAsync(cancellationToken);

                if (outcome.OutcomeStatus == OutcomeStatus.Approved &&
                    (outcome.OutcomeType is OutcomeType.WriteOff or OutcomeType.Usage))
                {
                    await outcomeService.UpdateBalanceAsync(outcome, cancellationToken);
                }

                if (outcome.OutcomeStatus == OutcomeStatus.Approved &&
                    (outcome.OutcomeType is OutcomeType.Broadcast or OutcomeType.Return))
                {
                    var incomeOutcome = await dbContext.IncomeOutcomes.FirstOrDefaultAsync(f => f.OutcomeId == outcome.Id);
                    if (incomeOutcome != null)
                    {
                        incomeOutcome.Income = outcomeService.CreateIncome(outcome, incomeOutcome.IncomeStockId);

                        var organizationSkus = outcomeService.CreateOrganizationSkus(outcome, incomeOutcome.IncomeStockId);
                        dbContext.OrganizationSkus.AddRange(organizationSkus);

                        dbContext.IncomeOutcomes.Update(incomeOutcome);
                        await dbContext.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                return outcome.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new AppException(ex, "Ошибка при изменение статуса расхода!");
            }
        });

        return outcomeId;
    }
}
