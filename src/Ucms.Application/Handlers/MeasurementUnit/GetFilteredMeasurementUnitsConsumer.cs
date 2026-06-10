namespace Ucms.Application.Handlers.MeasurementUnit;

using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetFilteredMeasurementUnitsMessage(PagedRequest Paging, string? Query) : IRequest<PagedResult<MeasurementUnitModel>>;

public class
    GetFilteredMeasurementUnitsConsumer : RequestHandler<GetFilteredMeasurementUnitsMessage,
        PagedResult<MeasurementUnitModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetFilteredMeasurementUnitsConsumer(
        IAppDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<PagedResult<MeasurementUnitModel>> Handle(GetFilteredMeasurementUnitsMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.MeasurementUnits
            .Where(w => w.EmergencyType == _workContext.EmergencyType);

        if (!string.IsNullOrEmpty(message.Query))
        {
            var searchQuery = message.Query.ToLowerInvariant().Trim();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchQuery)
                || x.NameRu.ToLower().Contains(searchQuery)
                || x.NameKa!.ToLower().Contains(searchQuery)
                || x.NameEn!.ToLower().Contains(searchQuery));
        }

        return await query
            .OrderBy(c => c.Name)
            .ToPagedResultAsync<MeasurementUnit, MeasurementUnitModel>(message.Paging, _mapper, cancellationToken);
    }
}
