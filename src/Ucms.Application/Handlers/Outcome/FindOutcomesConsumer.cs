namespace Ucms.Application.Handlers.Outcome;

using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindOutcomesMessage(string Query) : IRequest<List<OutcomeModel>>;

public class FindOutcomesConsumer : RequestHandler<FindOutcomesMessage, List<OutcomeModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindOutcomesConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<List<OutcomeModel>> Handle(FindOutcomesMessage message, CancellationToken cancellationToken)
    {
        var query = message.Query.ToLower();

        var outcomes = await _dbContext.Outcomes
            .Include(i => i.Stock)
            .Where(a => a.Name.ToLower().Contains(query) ||
                        a.Note!.ToLower().Contains(query))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<OutcomeModel>>(outcomes);

        return result;
    }
}
