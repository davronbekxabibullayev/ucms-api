namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindMeasurementUnitMessage(string Code) : IRequest<MeasurementUnitModel>;

public class FindMeasurementUnitConsumer : RequestHandler<FindMeasurementUnitMessage, MeasurementUnitModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindMeasurementUnitConsumer(
        IAppDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<MeasurementUnitModel> Handle(FindMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var measurementUnit = await _dbContext.MeasurementUnits
            .FirstOrDefaultAsync(f => f.Code == message.Code, cancellationToken)
            ?? throw new NotFoundException($"Measurement unit with code: {message.Code}, not found!");

        return _mapper.Map<MeasurementUnitModel>(measurementUnit);
    }
}
