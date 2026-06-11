namespace Ucms.Application.Handlers.Stock;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record DeleteStocksMessage(Guid[] Ids) : IRequest<bool>;

public class DeleteStocksConsumer : RequestHandler<DeleteStocksMessage, bool>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public DeleteStocksConsumer(IUcmsDbContext dbContext, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
    }
    protected override async Task<bool> Handle(DeleteStocksMessage message, CancellationToken cancellationToken)
    {
        var query = _dbContext.Stocks
            .AsTracking()
            .Where(f => message.Ids.Contains(f.Id));

        var existInIncome = _dbContext.Incomes.Any(a => message.Ids.Contains(a.StockId));
        var existInOutcome = _dbContext.Outcomes.Any(a => message.Ids.Contains(a.StockId));
        var existInBalance = _dbContext.StockSkus.Any(a => message.Ids.Contains(a.StockId));
        var existInParent = _dbContext.Stocks.Any(a => message.Ids.Contains(a.ParentId ?? Guid.Empty));

        if (!existInIncome && !existInOutcome && !existInBalance && !existInParent)
        {
            if (!_workContext.IsAdmin)
                query = query.Where(f => f.OrganizationId == _workContext.TenantId);

            var stocks = await query.ToListAsync(cancellationToken);

            if (stocks.Count > 0)
                foreach (var sku in stocks)
                    sku.IsDeleted = true;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Склад используется в других таблицах");
    }
}
