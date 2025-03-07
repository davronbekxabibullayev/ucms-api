namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;
public record DeleteMeasurementUnitsMessage(Guid[] Ids) : IRequest<bool>;

public class DeleteMeasurementUnitsConsumer : RequestHandler<DeleteMeasurementUnitsMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteMeasurementUnitsConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteMeasurementUnitsMessage message, CancellationToken cancellationToken)
    {
        var measurementUnits = await _dbContext.MeasurementUnits
            .AsTracking()
            .Where(f => message.Ids.Contains(f.Id))
            .ToListAsync(cancellationToken);

        var existInSku = _dbContext.Skus.Any(a => message.Ids.Contains(a.MeasurementUnitId));
        var existInDemand = _dbContext.StockDemandItems.Any(a => message.Ids.Contains(a.MeasurementUnitId));

        if (!existInSku && !existInDemand)
        {
            if (measurementUnits.Count > 0)
                foreach (var measurementUnit in measurementUnits)
                    measurementUnit.IsDeleted = true;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Единица складского учета используется в других таблицах");
    }
}
