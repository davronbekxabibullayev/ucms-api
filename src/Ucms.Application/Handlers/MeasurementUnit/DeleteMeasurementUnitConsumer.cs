namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteMeasurementUnitMessage(Guid Id) : IRequest<bool>;

public class DeleteMeasurementUnitConsumer : RequestHandler<DeleteMeasurementUnitMessage, bool>
{
    private readonly IUcmsDbContext _dbContext;

    public DeleteMeasurementUnitConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var measurementUnit = await _dbContext.MeasurementUnits
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException();

        measurementUnit.IsDeleted = true;
        var result = await _dbContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}
