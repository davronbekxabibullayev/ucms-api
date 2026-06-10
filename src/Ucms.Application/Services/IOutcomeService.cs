namespace Ucms.Application.Services;

using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;

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
