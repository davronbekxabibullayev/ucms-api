namespace Ucms.Application.Handlers.StockSku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record CheckForSkuAmountMessage(
    Guid SkuId,
    Guid StockId,
    Guid MeasurementUnitId,
    decimal Amount
) : IRequest<bool>;

public class CheckForSkuAmountConsumer : RequestHandler<CheckForSkuAmountMessage, bool>
{
    private readonly IUcmsDbContext _dbContext;

    public CheckForSkuAmountConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<bool> Handle(CheckForSkuAmountMessage message, CancellationToken cancellationToken)
    {
        var measurementUnit = await _dbContext.MeasurementUnits
            .FirstOrDefaultAsync(f => f.Id == message.MeasurementUnitId, cancellationToken);

        if (measurementUnit == null)
            return false;

        var amount = message.Amount * measurementUnit.Multiplier;
        var exist = await _dbContext.StockSkus.AnyAsync(f => f.StockId == message.StockId
                                                          && f.SkuId == message.SkuId
                                                          && f.Amount >= amount, cancellationToken);

        return exist;
    }
}
