namespace Ucms.Stock.Api.Application.Services;

using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;

public interface IOutcomeService
{
    Task UpdateBalanceAsync(Outcome outcome, CancellationToken cancellationToken);
    Task UpdateIncomeOutcome(Guid incomeId, CancellationToken cancellationToken);
    Task CancelIncomeOutcome(Guid incomeId, CancellationToken cancellationToken);
    IncomeOutcome CreateIncomeOutcome(Outcome outcome, Guid incomeStockId);
    Income CreateIncome(Outcome outcome, Guid incomeStockId);
    IEnumerable<OrganizationSku> CreateOrganizationSkus(Outcome outcome, Guid incomeStockId);
    Task ValidateOutcomeItems(IEnumerable<CreateOutcomeItemModel> outcomeItems, Guid stockId, CancellationToken cancellationToken);
    Task ValidateOutcomeItems(IEnumerable<OutcomeItem> outcomeItems, Guid stockId, CancellationToken cancellationToken);
}
