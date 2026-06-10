namespace Ucms.Stock.Api.Application.Consumers.Product;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateProductMessage
(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string? Code,
    string? InternationalCode,
    string? InternationalName,
    string? AlternativeName,
    ProductType Type) : IRequest<Guid>;
public class UpdateProductConsumer : RequestHandler<UpdateProductMessage, Guid>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public UpdateProductConsumer(IStockDbContext dbContext, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
    }
    protected override async Task<Guid> Handle(UpdateProductMessage message, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with ID: {message.Id}, not found");

        product.Name = message.Name;
        product.NameEn = message.NameEn;
        product.NameKa = message.NameKa;
        product.NameRu = message.NameRu;
        product.Code = message.Code;
        product.InternationalCode = message.InternationalCode;
        product.InternationalName = message.InternationalName;
        product.AlternativeName = message.AlternativeName;
        product.Type = message.Type;
        product.EmergencyType = _workContext.EmergencyType ?? EmergencyServiceType.Ambulance;

        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}

