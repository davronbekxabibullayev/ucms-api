namespace Ucms.Application.Handlers.Outcome;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteOutcomeMessage(Guid Id) : IRequest<bool>;
public class DeleteOutcomeConsumer : RequestHandler<DeleteOutcomeMessage, bool>
{
    private readonly IUcmsDbContext _dbContext;

    public DeleteOutcomeConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<bool> Handle(DeleteOutcomeMessage message, CancellationToken cancellationToken)
    {
        var outcome = await _dbContext.Outcomes
            .Include(i => i.OutcomeItems)
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Outcome), message.Id);

        foreach (var outcomeItem in outcome.OutcomeItems)
            outcomeItem.IsDeleted = true;

        outcome.IsDeleted = true;
        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
