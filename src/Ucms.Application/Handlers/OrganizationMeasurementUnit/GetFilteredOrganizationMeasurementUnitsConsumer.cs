namespace Ucms.Application.Handlers.OrganizationMeasurementUnit;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetFilteredOrganizationMeasurementUnitsMessage(PagedRequest Paging) : IRequest<PagedResult<OrganizationMeasurementUnitModel>>;

public class GetFilteredOrganizationMeasurementUnitsConsumer : RequestHandler<GetFilteredOrganizationMeasurementUnitsMessage,
        PagedResult<OrganizationMeasurementUnitModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetFilteredOrganizationMeasurementUnitsConsumer(
        IAppDbContext dbContext,
        IWorkContext workContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<OrganizationMeasurementUnitModel>> Handle(GetFilteredOrganizationMeasurementUnitsMessage message,
        CancellationToken cancellationToken)
    {
        return await _dbContext.OrganizationMeasurementUnits
            .Include(i => i.MeasurementUnit)
            .Where(w => w.OrganizationId == _workContext.TenantId)
            .ToPagedResultAsync<OrganizationMeasurementUnit, OrganizationMeasurementUnitModel>(message.Paging, _mapper, cancellationToken);
    }
}
