namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetOutcomeMessage(Guid Id) : IRequest<OutcomeModel>;

public class GetOutcomeConsumer : RequestHandler<GetOutcomeMessage, OutcomeModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetOutcomeConsumer(IStockDbContext dbContext, IMapper mapper)
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
