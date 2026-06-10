namespace Ucms.Stock.Api.Application.Consumers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;
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
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public UpdateSkuConsumer(IStockDbContext dbContext, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
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
        sku.EmergencyType = _workContext.EmergencyType ?? EmergencyServiceType.Ambulance;
        sku.Status = message.Status;

        _dbContext.Skus.Update(sku);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return sku.Id;
    }
}
