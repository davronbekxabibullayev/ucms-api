namespace Ucms.Stock.Api.Application.Consumers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;
public record DeleteSkuMessage(Guid Id) : IRequest<bool>;

public class DeleteSkuConsumer : RequestHandler<DeleteSkuMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteSkuConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteSkuMessage message, CancellationToken cancellationToken)
    {
        var sku = await _dbContext.Skus
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException();

        var existInIncome = _dbContext.IncomeItems.Any(a => a.SkuId == message.Id);
        var existInOutcome = _dbContext.OutcomeItems.Any(a => a.SkuId == message.Id);
        var existInBalance = _dbContext.StockSkus.Any(a => a.SkuId == message.Id);

        if (!existInIncome && !existInOutcome && !existInBalance)
        {
            sku.IsDeleted = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Единица складского учета используется в других таблицах");
    }
}
