namespace Ucms.Application.Handlers.Product;

using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record CreateProductMessage(
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string? Code,
    string? InternationalCode,
    string? InternationalName,
    string? AlternativeName,
    ProductType Type) : IRequest<Guid>;

public class CreateProductConsumer : RequestHandler<CreateProductMessage, Guid>
{
    private readonly IAppDbContext _dbContext;

    public CreateProductConsumer(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<Guid> Handle(CreateProductMessage message, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(f => f.Code == message.Code, cancellationToken);

        if (product != null)
            throw new AlreadyExistException($"Product with Code: {message.Code}, already exist");

        product = new Product
        {
            Name = message.Name,
            NameEn = message.NameEn,
            NameKa = message.NameKa,
            NameRu = message.NameRu,
            Code = message.Code,
            InternationalCode = message.InternationalCode,
            InternationalName = message.InternationalName,
            AlternativeName = message.AlternativeName,
            Type = message.Type
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
