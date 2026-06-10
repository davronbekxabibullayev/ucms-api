namespace Ucms.Application.Handlers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetMeasurementUnitsMessage : IRequest<List<MeasurementUnitModel>>;

public class GetMeasurementUnitsConsumer : RequestHandler<GetMeasurementUnitsMessage, List<MeasurementUnitModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetMeasurementUnitsConsumer(
        IAppDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<List<MeasurementUnitModel>> Handle(GetMeasurementUnitsMessage message, CancellationToken cancellationToken)
    {
        var measurementUnits = await _dbContext.MeasurementUnits
            .Where(w => w.EmergencyType == _workContext.EmergencyType)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<MeasurementUnitModel>>(measurementUnits);

        return result;
    }
}
