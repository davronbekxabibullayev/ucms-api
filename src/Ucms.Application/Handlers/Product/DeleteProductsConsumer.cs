namespace Ucms.Application.Handlers.Product;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteProductsMessage(Guid[] Ids) : IRequest<bool>;

public class DeleteProductsConsumer : RequestHandler<DeleteProductsMessage, bool>
{
    private readonly IUcmsDbContext _dbContext;

    public DeleteProductsConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteProductsMessage message, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products
            .AsTracking()
            .Where(f => message.Ids.Contains(f.Id))
            .ToListAsync(cancellationToken);

        var existInSku = _dbContext.Skus.Any(a => message.Ids.Contains(a.ProductId));
        var existInDemand = _dbContext.StockDemandItems.Any(a => message.Ids.Contains(a.ProductId));

        if (!existInSku && !existInDemand)
        {
            if (products.Count > 0)
                foreach (var sku in products)
                    sku.IsDeleted = true;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Продукт используется в других таблицах");
    }
}

