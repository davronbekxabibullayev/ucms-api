namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record FindOutcomeMessage(string Name) : IRequest<OutcomeModel>;

public class FindOutcomeConsumer : RequestHandler<FindOutcomeMessage, OutcomeModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindOutcomeConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<OutcomeModel> Handle(FindOutcomeMessage message, CancellationToken cancellationToken)
    {
        var outcome = await _dbContext.Outcomes
            .Include(a => a.OutcomeItems)
            .ThenInclude(a => a.Sku!.Product)
            .Include(i => i.OutcomeItems)
            .ThenInclude(th => th.MeasurementUnit)
            .Include(i => i.IncomeOutcome)
            .FirstOrDefaultAsync(f => f.Name == message.Name, cancellationToken)
            ?? throw new NotFoundException($"Outcome with Name: {message.Name}, not found!");

        return _mapper.Map<OutcomeModel>(outcome);
    }
}
