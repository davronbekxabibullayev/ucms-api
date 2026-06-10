namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetMeasurementUnitMessage(Guid Id) : IRequest<MeasurementUnitModel>;

public class GetMeasurementUnitConsumer : RequestHandler<GetMeasurementUnitMessage, MeasurementUnitModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetMeasurementUnitConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<MeasurementUnitModel> Handle(GetMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var measurementUnit = await _dbContext.MeasurementUnits
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Measurement unit with ID: {message.Id}, not found!");

        return _mapper.Map<MeasurementUnitModel>(measurementUnit);
    }
}
