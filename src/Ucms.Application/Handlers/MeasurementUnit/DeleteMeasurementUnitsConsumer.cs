namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteMeasurementUnitsMessage(Guid[] Ids) : IRequest<bool>;

public class DeleteMeasurementUnitsConsumer : RequestHandler<DeleteMeasurementUnitsMessage, bool>
{
    private readonly IAppDbContext _dbContext;

    public DeleteMeasurementUnitsConsumer(IAppDbContext dbContext)
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
