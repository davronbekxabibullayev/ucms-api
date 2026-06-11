namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record UpdateSkuMessage(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string SerialNumber,
    Guid ProductId,
    Guid? ManufacturerId,
    Guid MeasurementUnitId,
    Guid? SupplierId,
    decimal Price,
    decimal Amount,
    DateTimeOffset ExpirationDate,
    SkuStatus Status
) : IRequest<Guid>;

public class UpdateSkuConsumer : RequestHandler<UpdateSkuMessage, Guid>
{
    private readonly IUcmsDbContext _dbContext;

    public UpdateSkuConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<Guid> Handle(UpdateSkuMessage message, CancellationToken cancellationToken)
    {
        var sku = await _dbContext.Skus
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Sku with ID: {message.Id}, not found!");

        sku.Name = message.Name;
        sku.NameEn = message.NameEn;
        sku.NameKa = message.NameKa;
        sku.NameRu = message.NameRu;
        sku.SerialNumber = message.SerialNumber;
        sku.Price = message.Price;
        sku.Amount = message.Amount;
        sku.ExpirationDate = message.ExpirationDate;
        sku.ProductId = message.ProductId;
        sku.ManufacturerId = message.ManufacturerId;
        sku.MeasurementUnitId = message.MeasurementUnitId;
        sku.SupplierId = message.SupplierId;
        sku.Status = message.Status;

        _dbContext.Skus.Update(sku);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return sku.Id;
    }
}
