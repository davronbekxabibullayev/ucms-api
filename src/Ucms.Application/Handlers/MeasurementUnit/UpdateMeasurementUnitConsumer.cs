namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

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
    private readonly IUcmsDbContext _dbContext;

    public UpdateMeasurementUnitConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
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
