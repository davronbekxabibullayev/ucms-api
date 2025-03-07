namespace Ucms.Stock.Api.Application.Consumers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record CheckSkuForUsedMessage(Guid Id) : IRequest<bool>;

public class CheckSkuForUsedConsumer(IStockDbContext dbContext) : RequestHandler<CheckSkuForUsedMessage, bool>
{
    protected override async Task<bool> Handle(CheckSkuForUsedMessage message, CancellationToken cancellationToken)
    {
        var existInIncome = await dbContext.IncomeItems.AnyAsync(a => a.SkuId == message.Id, cancellationToken);
        var existInOutcome = await dbContext.OutcomeItems.AnyAsync(a => a.SkuId == message.Id, cancellationToken);
        var existInBalance = await dbContext.StockSkus.AnyAsync(a => a.SkuId == message.Id && a.Amount > 0, cancellationToken);

        var used = existInIncome || existInOutcome || existInBalance;

        return used;
    }
}
