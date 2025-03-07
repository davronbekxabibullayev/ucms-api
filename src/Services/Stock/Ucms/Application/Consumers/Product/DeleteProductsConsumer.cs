namespace Ucms.Stock.Api.Application.Consumers.Product;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record DeleteProductsMessage(Guid[] Ids) : IRequest<bool>;

public class DeleteProductsConsumer : RequestHandler<DeleteProductsMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteProductsConsumer(IStockDbContext dbContext)
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

