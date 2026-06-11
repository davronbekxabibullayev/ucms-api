namespace Ucms.Application.Handlers.OrganizationMeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetOrganizationMeasurementUnitMessage(Guid Id) : IRequest<OrganizationMeasurementUnitModel>;

public class GetOrganizationMeasurementUnitConsumer : RequestHandler<GetOrganizationMeasurementUnitMessage, OrganizationMeasurementUnitModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetOrganizationMeasurementUnitConsumer(
        IUcmsDbContext dbContext,
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
