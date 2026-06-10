namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

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
    private readonly IAppDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public CreateSkuConsumer(IAppDbContext dbContext, IWorkContext workContext)
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
            OrganizationId = _workContext.TenantId.Value,
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
                Status = message.Status
            }
        };

        _dbContext.OrganizationSkus.Add(organizationSku);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return organizationSku.SkuId;
    }
}
