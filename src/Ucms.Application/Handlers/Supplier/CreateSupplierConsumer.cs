namespace Ucms.Application.Handlers.Supplier;

using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record CreateSupplierMessage(
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string Code) : IRequest<Guid>;

public class CreateSupplierConsumer : RequestHandler<CreateSupplierMessage, Guid>
{
    private readonly IAppDbContext _dbContext;

    public CreateSupplierConsumer(IAppDbContext dbContext)
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
