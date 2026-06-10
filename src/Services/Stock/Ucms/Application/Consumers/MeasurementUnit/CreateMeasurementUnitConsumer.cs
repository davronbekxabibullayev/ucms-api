namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

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
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public CreateMeasurementUnitConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
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
            NameRu = message.NameRu,
            EmergencyType = _workContext.EmergencyType ?? EmergencyServiceType.Ambulance
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
