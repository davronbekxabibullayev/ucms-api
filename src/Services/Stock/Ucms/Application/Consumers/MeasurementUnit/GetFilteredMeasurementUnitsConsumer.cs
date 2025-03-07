namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using AutoMapper;
using Ucms.Common.Paging;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredMeasurementUnitsMessage(PagingRequest Paging, string? Query) : IRequest<PagedList<MeasurementUnitModel>>;

public class
    GetFilteredMeasurementUnitsConsumer : RequestHandler<GetFilteredMeasurementUnitsMessage,
        PagedList<MeasurementUnitModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetFilteredMeasurementUnitsConsumer(
        IStockDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<PagedList<MeasurementUnitModel>> Handle(GetFilteredMeasurementUnitsMessage message,
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
            .ToPagedListAsync<MeasurementUnit, MeasurementUnitModel>(message.Paging, _mapper);
    }
}
