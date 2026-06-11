namespace Ucms.Application.Handlers.Outcome;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetOutcomesMessage : IRequest<List<OutcomeModel>>;

public class GetOutcomesConsumer : RequestHandler<GetOutcomesMessage, List<OutcomeModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetOutcomesConsumer(IUcmsDbContext dbContext, IWorkContext workContext, IMapper mapper)
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
