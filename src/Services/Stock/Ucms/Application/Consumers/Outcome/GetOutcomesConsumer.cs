namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetOutcomesMessage : IRequest<List<OutcomeModel>>;

public class GetOutcomesConsumer : RequestHandler<GetOutcomesMessage, List<OutcomeModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetOutcomesConsumer(IStockDbContext dbContext, IWorkContext workContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
    }
    protected override async Task<List<OutcomeModel>> Handle(GetOutcomesMessage message, CancellationToken cancellationToken)
    {
        var outcomes = await _dbContext.Outcomes
            .Include(i => i.Stock)
            .Where(w => w.Stock!.OrganizationId == _workContext.TenantId)
            .OrderByDescending(a => a.OutcomeDate)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<OutcomeModel>>(outcomes);

        return result;
    }
}
