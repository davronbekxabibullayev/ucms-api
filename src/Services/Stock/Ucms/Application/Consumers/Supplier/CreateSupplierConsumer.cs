namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record CreateSupplierMessage(
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string Code) : IRequest<Guid>;

public class CreateSupplierConsumer : RequestHandler<CreateSupplierMessage, Guid>
{
    private readonly IStockDbContext _dbContext;

    public CreateSupplierConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<Guid> Handle(CreateSupplierMessage message, CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers
            .FirstOrDefaultAsync(f => f.Code == message.Code, cancellationToken);

        if (supplier != null)
            throw new AlreadyExistException($"Supplier with Code: {message.Code}, already exist");

        supplier = new Supplier
        {
            Name = message.Name,
            NameEn = message.NameEn,
            NameKa = message.NameKa,
            NameRu = message.NameRu,
            Code = message.Code
        };

        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
