namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetExecutionOutcomeMessage(Guid ExecutionId) : IRequest<OutcomeModel>;

public class GetExecutionOutcomeConsumer : RequestHandler<GetExecutionOutcomeMessage, OutcomeModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetExecutionOutcomeConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<OutcomeModel> Handle(GetExecutionOutcomeMessage message, CancellationToken cancellationToken)
    {
        var outcome = await _dbContext.Outcomes
           .Include(i => i.OutcomeItems)
           .ThenInclude(th => th.Sku!.Product)
           .Include(i => i.OutcomeItems)
           .ThenInclude(th => th.MeasurementUnit)
           .Include(i => i.IncomeOutcome)
           .FirstOrDefaultAsync(f => f.ExecutionId == message.ExecutionId, cancellationToken)
           ?? throw new NotFoundException($"Outcome with ExecutionId: {message.ExecutionId}, not found!");

        return _mapper.Map<OutcomeModel>(outcome);
    }
}
