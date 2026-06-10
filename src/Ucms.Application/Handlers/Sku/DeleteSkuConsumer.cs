namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteSkuMessage(Guid Id) : IRequest<bool>;

public class DeleteSkuConsumer : RequestHandler<DeleteSkuMessage, bool>
{
    private readonly IAppDbContext _dbContext;

    public DeleteSkuConsumer(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteSkuMessage message, CancellationToken cancellationToken)
    {
        var sku = await _dbContext.Skus
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException();

        var existInIncome = _dbContext.IncomeItems.Any(a => a.SkuId == message.Id);
        var existInOutcome = _dbContext.OutcomeItems.Any(a => a.SkuId == message.Id);
        var existInBalance = _dbContext.StockSkus.Any(a => a.SkuId == message.Id);

        if (!existInIncome && !existInOutcome && !existInBalance)
        {
            sku.IsDeleted = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Единица складского учета используется в других таблицах");
    }
}
