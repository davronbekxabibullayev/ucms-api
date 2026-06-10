namespace Ucms.Application.Handlers.Supplier;

using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteSupplierMessage(Guid Id) : IRequest<bool>;

public class DeleteSupplierConsumer : RequestHandler<DeleteSupplierMessage, bool>
{
    private readonly IAppDbContext _dbContext;

    public DeleteSupplierConsumer(IAppDbContext dbContext)
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
