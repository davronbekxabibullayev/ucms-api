namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateSupplierMessage
(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string Code) : IRequest<Guid>;
public class UpdateSupplierConsumer : RequestHandler<UpdateSupplierMessage, Guid>
{
    private readonly IStockDbContext _dbContext;

    public UpdateSupplierConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<Guid> Handle(UpdateSupplierMessage message, CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Supplier with ID: {message.Id}, not found");

        supplier.Name = message.Name;
        supplier.NameEn = message.NameEn;
        supplier.NameKa = message.NameKa;
        supplier.NameRu = message.NameRu;
        supplier.Code = message.Code;

        _dbContext.Suppliers.Update(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}

