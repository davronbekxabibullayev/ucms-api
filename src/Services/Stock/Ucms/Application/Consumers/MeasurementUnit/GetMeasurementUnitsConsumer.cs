namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetMeasurementUnitsMessage : IRequest<List<MeasurementUnitModel>>;

public class GetMeasurementUnitsConsumer : RequestHandler<GetMeasurementUnitsMessage, List<MeasurementUnitModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetMeasurementUnitsConsumer(
        IStockDbContext dbContext,
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
