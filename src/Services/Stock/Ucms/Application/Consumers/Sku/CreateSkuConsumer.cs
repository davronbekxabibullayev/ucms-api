namespace Ucms.Stock.Api.Application.Consumers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record CreateSkuMessage(
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

public class CreateSkuConsumer : RequestHandler<CreateSkuMessage, Guid>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public CreateSkuConsumer(IStockDbContext dbContext, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
    }
    protected override async Task<Guid> Handle(CreateSkuMessage message, CancellationToken cancellationToken)
    {
        var exist = await _dbContext.Skus
           .AnyAsync(f => f.SerialNumber == message.SerialNumber, cancellationToken);

        if (exist)
            throw new AlreadyExistException($"Sku with serial number: {message.SerialNumber}, already exist!");

        var organizationSku = new OrganizationSku
        {
            OrganizationId = _workContext.TenantId,
            Sku = new Sku
            {
                Name = message.Name,
                NameEn = message.NameEn,
                NameKa = message.NameKa,
                NameRu = message.NameRu,
                SerialNumber = message.SerialNumber,
                Price = message.Price,
                Amount = message.Amount,
                ExpirationDate = message.ExpirationDate,
                ProductId = message.ProductId,
                ManufacturerId = message.ManufacturerId,
                MeasurementUnitId = message.MeasurementUnitId,
                SupplierId = message.SupplierId,
                EmergencyType = _workContext.EmergencyType ?? EmergencyServiceType.Ambulance,
                Status = message.Status
            }
        };

        _dbContext.OrganizationSkus.Add(organizationSku);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return organizationSku.SkuId;
    }
}
