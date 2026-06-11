namespace Ucms.Application.Handlers.Income;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteIncomeMessage(Guid Id) : IRequest<bool>;
public class DeleteIncomeConsumer : RequestHandler<DeleteIncomeMessage, bool>
{
    private readonly IUcmsDbContext _dbContext;

    public DeleteIncomeConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<bool> Handle(DeleteIncomeMessage message, CancellationToken cancellationToken)
    {
        var income = await _dbContext.Incomes
            .Include(i => i.IncomeItems)
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Income), message.Id);

        foreach (var incomeItem in income.IncomeItems)
            incomeItem.IsDeleted = true;

        income.IsDeleted = true;
        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
