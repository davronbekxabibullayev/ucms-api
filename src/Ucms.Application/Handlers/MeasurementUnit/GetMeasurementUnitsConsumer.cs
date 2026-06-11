namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetMeasurementUnitsMessage : IRequest<List<MeasurementUnitModel>>;

public class GetMeasurementUnitsConsumer : RequestHandler<GetMeasurementUnitsMessage, List<MeasurementUnitModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetMeasurementUnitsConsumer(
        IUcmsDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<List<MeasurementUnitModel>> Handle(GetMeasurementUnitsMessage message, CancellationToken cancellationToken)
    {
        var measurementUnits = await _dbContext.MeasurementUnits
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<MeasurementUnitModel>>(measurementUnits);

        return result;
    }
}
