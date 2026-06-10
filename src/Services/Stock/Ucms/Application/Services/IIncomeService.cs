namespace Ucms.Stock.Api.Application.Services;

using Ucms.Stock.Domain.Models;

public interface IIncomeService
{
    Task UpdateBalanceAsync(Income income, CancellationToken cancellationToken);
}
