namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record DeleteSupplierMessage(Guid Id) : IRequest<bool>;

public class DeleteSupplierConsumer : RequestHandler<DeleteSupplierMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteSupplierConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<bool> Handle(DeleteSupplierMessage message, CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Supplier with id : {message.Id} is not found!");

        var existInSku = _dbContext.Skus.Any(a => a.SupplierId == message.Id);

        if (!existInSku)
        {
            supplier.IsDeleted = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Поставщик используется в других таблицах");
    }
}
