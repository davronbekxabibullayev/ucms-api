namespace Ucms.Application.Handlers.StockSku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetProductBalanceMessage(
    Guid StockId,
    Guid ProductId,
    Guid MeasurementUnitId
) : IRequest<decimal>;

public class GetProductBalanceConsumer : RequestHandler<GetProductBalanceMessage, decimal>
{
    private readonly IAppDbContext _dbContext;

    public GetProductBalanceConsumer(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<decimal> Handle(GetProductBalanceMessage message, CancellationToken cancellationToken)
    {
        var balance = await _dbContext.StockSkus
            .FirstOrDefaultAsync(w => w.StockId == message.StockId
                     && w.MeasurementUnitId == message.MeasurementUnitId
                     && w.Sku!.ProductId == message.ProductId,
                     cancellationToken);

        if (balance == null)
            return 0;

        return balance.Amount;
    }
}
