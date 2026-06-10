namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record DeleteSuppliersMessage(Guid[] Ids) : IRequest<bool>;

public class DeleteSuppliersConsumer : RequestHandler<DeleteSuppliersMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteSuppliersConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteSuppliersMessage message, CancellationToken cancellationToken)
    {
        var suppliers = await _dbContext.Suppliers
            .AsTracking()
            .Where(f => message.Ids.Contains(f.Id))
            .ToListAsync(cancellationToken);

        var existInSku = _dbContext.Skus.Any(a => message.Ids.Contains(a.SupplierId ?? Guid.Empty));

        if (!existInSku)
        {
            suppliers.ForEach(f => f.IsDeleted = true);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Поставщик используется в других таблицах");
    }
}

