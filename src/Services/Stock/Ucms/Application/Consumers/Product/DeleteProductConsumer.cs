namespace Ucms.Stock.Api.Application.Consumers.Product;

using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record DeleteProductMessage(Guid Id) : IRequest<bool>;

public class DeleteProductConsumer : RequestHandler<DeleteProductMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteProductConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<bool> Handle(DeleteProductMessage message, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with id : {message.Id} is not found!");

        var existInSku = _dbContext.Skus.Any(a => a.ProductId == message.Id);
        var existInDemand = _dbContext.StockDemandItems.Any(a => a.ProductId == message.Id);

        if (!existInSku && !existInDemand)
        {
            product.IsDeleted = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Продукт используется в других таблицах");
    }
}
