namespace Ucms.Application.Handlers.OrganizationMeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetOrganizationMeasurementUnitsMessage : IRequest<List<OrganizationMeasurementUnitModel>>;

public class GetOrganizationMeasurementUnitsConsumer : RequestHandler<GetOrganizationMeasurementUnitsMessage, List<OrganizationMeasurementUnitModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetOrganizationMeasurementUnitsConsumer(
        IUcmsDbContext dbContext,
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
