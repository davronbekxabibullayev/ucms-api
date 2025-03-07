namespace Ucms.Stock.Api.Application.Consumers.Income;

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

public record UpdateIncomeMessage(
    Guid Id,
    string Name,
    string? Note,
    IncomeType IncomeType,
    IncomeStatus IncomeStatus,
    PaymentType PaymentType,
    DateTimeOffset IncomeDate,
    Guid StockId,
    IEnumerable<CreateIncomeItemModel> IncomeItems
) : IRequest<Guid>;

public class UpdateIncomeConsumer(
    IStockDbContext dbContext,
    IIncomeService incomeService,
    IOutcomeService outcomeService,
    IWorkContext workContext,
    IOrganizationService organizationService,
    ILogger<UpdateIncomeConsumer> logger) : RequestHandler<UpdateIncomeMessage, Guid>
{
    protected override async Task<Guid> Handle(UpdateIncomeMessage message, CancellationToken cancellationToken)
    {
        var income = await dbContext.Incomes.Include(i => i.IncomeItems)
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Income with ID: {message.Id}, not found!");

        var strategy = dbContext.CreateExecutionStrategy();
        var incomeId = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                dbContext.IncomeItems.RemoveRange(income.IncomeItems);
                await dbContext.SaveChangesAsync(cancellationToken);

                await UpdateIncome(income, message);

                dbContext.Incomes.Update(income);
                await dbContext.SaveChangesAsync(cancellationToken);

                if (income.IncomeTransferStatus == IncomeTransferStatus.Received)
                {
                    if (income.IncomeStatus == IncomeStatus.Approved)
                        income.IncomeTransferStatus = IncomeTransferStatus.Approved;
                    else if (income.IncomeStatus == IncomeStatus.Cancelled)
                        income.IncomeTransferStatus = IncomeTransferStatus.Cancelled;
                }

                if (income.IncomeStatus == IncomeStatus.Approved)
                {
                    await incomeService.UpdateBalanceAsync(income, cancellationToken);

                    await outcomeService.UpdateIncomeOutcome(income.Id, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return income.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError("Error on updating income: {Message}", ex.Message);
                throw new AppException(ex.Message);
            }
        });

        return incomeId;
    }

    private async Task UpdateIncome(Income income, UpdateIncomeMessage message)
    {
        income.Name = message.Name;
        income.Note = message.Note;
        income.IncomeType = message.IncomeType;
        income.IncomeStatus = message.IncomeStatus;
        income.PaymentType = message.PaymentType;
        income.IncomeDate = message.IncomeDate;
        income.StockId = message.StockId;
        income.EmployeeId = workContext.EmployeeId;
        income.EmployeeName = await organizationService.GetEmployeeName(workContext.EmployeeId);
        income.IncomeItems = message.IncomeItems.Select(s => new IncomeItem
        {
            Amount = s.Amount,
            SkuId = s.SkuId,
            MeasurementUnitId = s.MeasurementUnitId,
        }).ToArray();
    }
}
