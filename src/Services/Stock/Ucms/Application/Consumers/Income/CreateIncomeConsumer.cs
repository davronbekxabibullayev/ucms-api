namespace Ucms.Stock.Api.Application.Consumers.Income;

using System.Threading;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Api.Application.Services;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Stock.Contracts.Models;

public record CreateIncomeMessage(
    string Name,
    string? Note,
    IncomeType IncomeType,
    IncomeStatus IncomeStatus,
    PaymentType PaymentType,
    DateTimeOffset IncomeDate,
    Guid StockId,
    IEnumerable<CreateIncomeItemModel> IncomeItems) : IRequest<Guid>;

public class CreateIncomeConsumer(
    IStockDbContext dbContext,
    IIncomeService incomeService,
    IWorkContext workContext,
    ILogger<CreateIncomeConsumer> logger) : RequestHandler<CreateIncomeMessage, Guid>
{
    protected override async Task<Guid> Handle(CreateIncomeMessage message, CancellationToken cancellationToken)
    {
        var strategy = dbContext.CreateExecutionStrategy();
        var incomeId = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var income = await GetIncome(message);
                dbContext.Incomes.Add(income);
                await dbContext.SaveChangesAsync(cancellationToken);

                if (income.IncomeStatus == IncomeStatus.Approved)
                {
                    await incomeService.UpdateBalanceAsync(income, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return income.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError("Error on creating income: {Message}", ex.Message);
                throw new AppException(ex.Message);
            }
        });

        return incomeId;
    }

    private async Task<Income> GetIncome(CreateIncomeMessage message)
    {
        return new Income
        {
            Name = message.Name,
            Note = message.Note,
            IncomeType = message.IncomeType,
            IncomeStatus = message.IncomeStatus,
            PaymentType = message.PaymentType,
            IncomeDate = message.IncomeDate,
            StockId = message.StockId,
            EmployeeId = workContext.EmployeeId,
            EmployeeName = await organizationService.GetEmployeeName(workContext.EmployeeId),
            IncomeItems = message.IncomeItems.Select(s => new IncomeItem
            {
                Amount = s.Amount,
                SkuId = s.SkuId,
                MeasurementUnitId = s.MeasurementUnitId,
            }).ToArray()
        };
    }
}
