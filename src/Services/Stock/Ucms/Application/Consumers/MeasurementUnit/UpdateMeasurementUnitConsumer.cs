namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateMeasurementUnitMessage(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string? Code,
    MeasurementUnitType Type,
    decimal Multiplier) : IRequest<Guid>;

public class UpdateMeasurementUnitConsumer : RequestHandler<UpdateMeasurementUnitMessage, Guid>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public UpdateMeasurementUnitConsumer(IStockDbContext dbContext, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
    }
    protected override async Task<Guid> Handle(UpdateMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        await ValidateOrThrowAsync(message);

        var measurementUnit = await GetEntityOrThrowAsync(message, cancellationToken);

        UpdateEntity(message, measurementUnit);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return measurementUnit.Id;
    }

    private void UpdateEntity(UpdateMeasurementUnitMessage message, MeasurementUnit measurementUnit)
    {
        measurementUnit.Type = message.Type;
        measurementUnit.Multiplier = message.Multiplier;
        measurementUnit.Name = message.Name;
        measurementUnit.NameEn = message.NameEn;
        measurementUnit.NameKa = message.NameKa;
        measurementUnit.NameRu = message.NameRu;
        measurementUnit.EmergencyType = _workContext.EmergencyType ?? EmergencyServiceType.Ambulance;
    }

    private async Task<MeasurementUnit> GetEntityOrThrowAsync(UpdateMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        return await _dbContext.MeasurementUnits
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(MeasurementUnit), message.Id);
    }

    private async Task ValidateOrThrowAsync(UpdateMeasurementUnitMessage message)
    {
        var isExist = await _dbContext.MeasurementUnits.AnyAsync(a => a.Id != message.Id && a.Code == message.Code);

        if (isExist)
        {
            throw new AlreadyExistException(nameof(MeasurementUnit), message.Code!);
        }
    }
}
