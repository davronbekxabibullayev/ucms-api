namespace Ucms.Application.Handlers.Income;

using System.Threading;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Application.Services;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record UpdateIncomeStatusMessage(
    Guid Id,
    IncomeStatus Status
) : IRequest<Guid>;

public class UpdateIncomeStatusConsumer(
    IAppDbContext dbContext,
    IIncomeService incomeService,
    IOutcomeService outcomeService,
    ILogger<UpdateIncomeStatusConsumer> logger) : RequestHandler<UpdateIncomeStatusMessage, Guid>
{
    protected override async Task<Guid> Handle(UpdateIncomeStatusMessage message, CancellationToken cancellationToken)
    {
        var income = await dbContext.Incomes
            .Include(i => i.IncomeItems)
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Income with ID: {message.Id}, not found!");

        var strategy = dbContext.CreateExecutionStrategy();
        var incomeId = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                income.IncomeStatus = message.Status;
                if (income.IncomeTransferStatus == IncomeTransferStatus.Received)
                {
                    income.IncomeTransferStatus = GetIncomeTransferStatus(message);
                }
                dbContext.Incomes.Update(income);
                await dbContext.SaveChangesAsync(cancellationToken);

                if (income.IncomeStatus == IncomeStatus.Approved)
                {
                    await incomeService.UpdateBalanceAsync(income, cancellationToken);

                    await outcomeService.UpdateIncomeOutcome(income.Id, cancellationToken);
                }
                else if (income.IncomeStatus == IncomeStatus.Cancelled)
                {
                    await outcomeService.CancelIncomeOutcome(income.Id, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return income.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError(ex, "Error on updating income status.");

                throw new AppException(ex.Message);
            }
        });

        return incomeId;
    }

    private static IncomeTransferStatus GetIncomeTransferStatus(UpdateIncomeStatusMessage message)
    {
        return message.Status switch
        {
            IncomeStatus.Approved => IncomeTransferStatus.Approved,
            IncomeStatus.Cancelled => IncomeTransferStatus.Cancelled,
            IncomeStatus.Draft => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }
}
