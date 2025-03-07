namespace Ucms.Stock.Api.Application.Consumers.Product;

using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

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
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public CreateProductConsumer(IStockDbContext dbContext, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
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
            Type = message.Type,
            EmergencyType = _workContext.EmergencyType ?? EmergencyServiceType.Ambulance
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
