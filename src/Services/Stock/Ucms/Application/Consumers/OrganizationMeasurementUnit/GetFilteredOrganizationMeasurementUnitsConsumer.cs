namespace Ucms.Stock.Api.Application.Consumers.OrganizationMeasurementUnit;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Paging;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredOrganizationMeasurementUnitsMessage(PagingRequest Paging) : IRequest<PagedList<OrganizationMeasurementUnitModel>>;

public class GetFilteredOrganizationMeasurementUnitsConsumer : RequestHandler<GetFilteredOrganizationMeasurementUnitsMessage,
        PagedList<OrganizationMeasurementUnitModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetFilteredOrganizationMeasurementUnitsConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
    }

    protected override async Task<PagedList<OrganizationMeasurementUnitModel>> Handle(GetFilteredOrganizationMeasurementUnitsMessage message,
        CancellationToken cancellationToken)
    {
        return await _dbContext.OrganizationMeasurementUnits
            .Include(i => i.MeasurementUnit)
            .Where(w => w.OrganizationId == _workContext.TenantId)
            .ToPagedListAsync<OrganizationMeasurementUnit, OrganizationMeasurementUnitModel>(message.Paging, _mapper);
    }
}
