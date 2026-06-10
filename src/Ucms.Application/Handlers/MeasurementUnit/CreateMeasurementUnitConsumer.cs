namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record CreateMeasurementUnitMessage(
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string Code,
    decimal Multiplier,
    MeasurementUnitType Type) : IRequest<Guid>;

public class CreateMeasurementUnitConsumer : RequestHandler<CreateMeasurementUnitMessage, Guid>
{
    private readonly IAppDbContext _dbContext;

    public CreateMeasurementUnitConsumer(
        IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<Guid> Handle(CreateMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        await ValidateOrThrowAsync(message, cancellationToken);

        var measurementUnit = NewEntity(message);

        _dbContext.MeasurementUnits.Add(measurementUnit);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return measurementUnit.Id;
    }

    private MeasurementUnit NewEntity(CreateMeasurementUnitMessage message)
    {
        return new MeasurementUnit
        {
            Code = message.Code,
            Multiplier = message.Multiplier,
            Type = message.Type,
            Name = message.Name,
            NameEn = message.NameEn,
            NameKa = message.NameKa,
            NameRu = message.NameRu
        };
    }

    private async Task ValidateOrThrowAsync(CreateMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var exist = await _dbContext.MeasurementUnits
            .AnyAsync(f => f.Code == message.Code, cancellationToken);

        if (exist)
            throw new AlreadyExistException(nameof(MeasurementUnit), message.Code);
    }
}
