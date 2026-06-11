namespace Ucms.Application.Handlers.OrganizationMeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record UpsertOrganizationMeasurementUnitMessage(MeasurementUnitType Type, Guid MeasurementUnitId) : IRequest<Guid>;

public class UpsertOrganizationMeasurementUnitConsumer : RequestHandler<UpsertOrganizationMeasurementUnitMessage, Guid>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public UpsertOrganizationMeasurementUnitConsumer(
        IUcmsDbContext dbContext,
        IWorkContext workContext
    )
    {
        _dbContext = dbContext;
        _workContext = workContext;
    }

    protected override async Task<Guid> Handle(UpsertOrganizationMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var organizationMeasurementUnit = await _dbContext.OrganizationMeasurementUnits
            .AsTracking()
            .FirstOrDefaultAsync(f => f.OrganizationId == _workContext.TenantId && f.Type == message.Type, cancellationToken);

        if (organizationMeasurementUnit == null)
        {
            organizationMeasurementUnit = NewEntity(message);
            _dbContext.OrganizationMeasurementUnits.Add(organizationMeasurementUnit);
        }
        else
        {
            organizationMeasurementUnit.MeasurementUnitId = message.MeasurementUnitId;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        return organizationMeasurementUnit.Id;
    }

    private OrganizationMeasurementUnit NewEntity(UpsertOrganizationMeasurementUnitMessage message)
    {
        return new OrganizationMeasurementUnit
        {
            Type = message.Type,
            OrganizationId = _workContext.TenantId ?? Guid.Empty,
            MeasurementUnitId = message.MeasurementUnitId
        };
    }
}
