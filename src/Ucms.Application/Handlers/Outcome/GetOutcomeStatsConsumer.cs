namespace Ucms.Application.Handlers.Outcome;

using System.Threading;
using AutoMapper;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;
using Ucms.Application.Abstractions.Organization;

public record GetOutcomeStatsMessage(
    Guid OrganizationId,
    DateTime From,
    DateTime To,
    DateTime PreviousFrom,
    DateTime PreviousTo) : IRequest<OutcomeStatsModel>;

public class GetOutcomeStatsConsumer : RequestHandler<GetOutcomeStatsMessage, OutcomeStatsModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IOrganizationClient _organizationClient;
    private readonly IMapper _mapper;

    public GetOutcomeStatsConsumer(IUcmsDbContext dbContext, IOrganizationClient organizationClient, IMapper mapper)
    {
        _dbContext = dbContext;
        _organizationClient = organizationClient;
        _mapper = mapper;
    }
    protected override async Task<OutcomeStatsModel> Handle(GetOutcomeStatsMessage message, CancellationToken cancellationToken)
    {
        var organizationIds = await _organizationClient.GetOrganizationIds(message.OrganizationId);

        var items = _dbContext.OutcomeItems.Where(w => organizationIds.Contains(w.Outcome!.Stock!.OrganizationId));

        var currentPeriodItems = items.Where(w => w.Outcome!.OutcomeDate > message.From
                                               && w.Outcome!.OutcomeDate < message.To);
        var previousPeriodItems = items.Where(w => w.Outcome!.OutcomeDate > message.PreviousFrom
                                                && w.Outcome!.OutcomeDate < message.PreviousTo);

        var currentPeriodData = currentPeriodItems.GroupBy(g => g.Sku)
                            .Select(s => new { Sku = s.Key, Count = s.Count() })
                            .OrderBy(o => o.Count)
                            .Take(10)
                            .ToList();
        var previousPeriodData = previousPeriodItems.GroupBy(g => g.Sku)
                            .Select(s => new { Sku = s.Key, Count = s.Count() })
                            .OrderBy(o => o.Count)
                            .Take(10)
                            .ToList();

        var currentPeriod = currentPeriodData.Select(p => new OutcomeStatItemModel(_mapper.Map<SkuModel>(p.Sku), p.Count));
        var previousPeriod = previousPeriodData.Select(p => new OutcomeStatItemModel(_mapper.Map<SkuModel>(p.Sku), p.Count));

        return new OutcomeStatsModel(currentPeriod.ToList(), previousPeriod.ToList());
    }
}
