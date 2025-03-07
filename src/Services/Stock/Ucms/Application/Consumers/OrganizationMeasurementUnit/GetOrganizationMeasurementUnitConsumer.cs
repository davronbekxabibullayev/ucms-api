namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetOrganizationMeasurementUnitMessage(Guid Id) : IRequest<OrganizationMeasurementUnitModel>;

public class GetOrganizationMeasurementUnitConsumer : RequestHandler<GetOrganizationMeasurementUnitMessage, OrganizationMeasurementUnitModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetOrganizationMeasurementUnitConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
    }
    protected override async Task<OrganizationMeasurementUnitModel> Handle(GetOrganizationMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var organizationMeasurementUnit = await _dbContext.OrganizationMeasurementUnits
            .Include(i => i.MeasurementUnit)
            .FirstOrDefaultAsync(f => f.Id == message.Id && f.OrganizationId == _workContext.TenantId, cancellationToken)
            ?? throw new NotFoundException($"Organization Measurement unit with ID: {message.Id}, not found!");

        return _mapper.Map<OrganizationMeasurementUnitModel>(organizationMeasurementUnit);
    }
}
