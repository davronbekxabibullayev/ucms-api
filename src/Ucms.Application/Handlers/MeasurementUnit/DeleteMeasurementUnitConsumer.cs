namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteMeasurementUnitMessage(Guid Id) : IRequest<bool>;

public class DeleteMeasurementUnitConsumer : RequestHandler<DeleteMeasurementUnitMessage, bool>
{
    private readonly IAppDbContext _dbContext;

    public DeleteMeasurementUnitConsumer(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var measurementUnit = await _dbContext.MeasurementUnits
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException();

        var existInSku = _dbContext.Skus.Any(a => a.MeasurementUnitId == message.Id);
        var existInDemand = _dbContext.StockDemandItems.Any(a => a.MeasurementUnitId == message.Id);

        if (!existInSku && !existInDemand)
        {
            measurementUnit.IsDeleted = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Единица измерения используется в других таблицах");
    }
}
