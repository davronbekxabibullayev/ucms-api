namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetOrganizationMeasurementUnitsMessage : IRequest<List<OrganizationMeasurementUnitModel>>;

public class GetOrganizationMeasurementUnitsConsumer : RequestHandler<GetOrganizationMeasurementUnitsMessage, List<OrganizationMeasurementUnitModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetOrganizationMeasurementUnitsConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
    }
    protected override async Task<List<OrganizationMeasurementUnitModel>> Handle(
        GetOrganizationMeasurementUnitsMessage message,
        CancellationToken cancellationToken)
    {
        var measurementUnits = await _dbContext.OrganizationMeasurementUnits
            .Include(i => i.MeasurementUnit)
            .Where(w => w.OrganizationId == _workContext.TenantId)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<OrganizationMeasurementUnitModel>>(measurementUnits);

        return result;
    }
}
