namespace Ucms.Application.Handlers.Outcome;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetOutcomeMessage(Guid Id) : IRequest<OutcomeModel>;

public class GetOutcomeConsumer : RequestHandler<GetOutcomeMessage, OutcomeModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetOutcomeConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<OutcomeModel> Handle(GetOutcomeMessage message, CancellationToken cancellationToken)
    {
        var outcome = await _dbContext.Outcomes
           .Include(i => i.OutcomeItems)
           .ThenInclude(th => th.Sku!.Product)
           .Include(i => i.OutcomeItems)
           .ThenInclude(th => th.Sku!.MeasurementUnit)
           .Include(i => i.OutcomeItems)
           .ThenInclude(th => th.MeasurementUnit)
           .Include(i => i.IncomeOutcome)
           .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
           ?? throw new NotFoundException($"Outcome with ID: {message.Id}, not found!");

        return _mapper.Map<OutcomeModel>(outcome);
    }
}
