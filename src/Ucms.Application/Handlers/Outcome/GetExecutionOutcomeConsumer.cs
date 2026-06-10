namespace Ucms.Application.Handlers.Outcome;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetExecutionOutcomeMessage(Guid ExecutionId) : IRequest<OutcomeModel>;

public class GetExecutionOutcomeConsumer : RequestHandler<GetExecutionOutcomeMessage, OutcomeModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetExecutionOutcomeConsumer(IAppDbContext dbContext, IMapper mapper)
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
