namespace Ucms.Application.Handlers.Income;

using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindIncomesMessage(string Query) : IRequest<List<IncomeModel>>;

public class FindIncomesConsumer : RequestHandler<FindIncomesMessage, List<IncomeModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindIncomesConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<List<IncomeModel>> Handle(FindIncomesMessage message, CancellationToken cancellationToken)
    {
        var query = message.Query.ToLower();

        var incomes = await _dbContext.Incomes
            .Where(a => a.Name!.ToLower().Contains(query) ||
                        a.Note!.ToLower().Contains(query))
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<IncomeModel>>(incomes);

        return result;
    }
}
