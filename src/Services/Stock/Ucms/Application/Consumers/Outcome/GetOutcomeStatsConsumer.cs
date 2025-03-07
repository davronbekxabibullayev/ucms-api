namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using System.Threading;
using AutoMapper;
using Ucms.Core.Services.Mediator;
using Ucms.Organization.Clients;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetOutcomeStatsMessage(
    Guid OrganizationId,
    DateTime From,
    DateTime To,
    DateTime PreviousFrom,
    DateTime PreviousTo) : IRequest<OutcomeStatsModel>;

public class GetOutcomeStatsConsumer : RequestHandler<GetOutcomeStatsMessage, OutcomeStatsModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IOrganizationClient _organizationClient;
    private readonly IMapper _mapper;

    public GetOutcomeStatsConsumer(IStockDbContext dbContext, IOrganizationClient organizationClient, IMapper mapper)
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
