namespace Ucms.Stock.Api.Application.Consumers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record DeleteSkusMessage(Guid[] Ids): IRequest<bool>;

public class DeleteSkusConsumer : RequestHandler<DeleteSkusMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteSkusConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<bool> Handle(DeleteSkusMessage message, CancellationToken cancellationToken)
    {
        var skus = await _dbContext.Skus
            .AsTracking()
            .Where(f => message.Ids.Contains(f.Id))
            .ToListAsync(cancellationToken);

        var existInIncome = _dbContext.IncomeItems.Any(a => message.Ids.Contains(a.SkuId));
        var existInOutcome = _dbContext.OutcomeItems.Any(a => message.Ids.Contains(a.SkuId));
        var existInBalance = _dbContext.StockSkus.Any(a => message.Ids.Contains(a.SkuId));

        if (!existInIncome && !existInOutcome && !existInBalance)
        {
            if (skus.Count > 0)
                foreach (var sku in skus)
                    sku.IsDeleted = true;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
        else
            throw new AppException("Единица складского учета используется в других таблицах");
    }
}
