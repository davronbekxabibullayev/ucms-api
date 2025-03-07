namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record DeleteManufacturerMessage(Guid Id) : IRequest<bool>;
public class DeleteManufacturerConsumer : RequestHandler<DeleteManufacturerMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteManufacturerConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<bool> Handle(DeleteManufacturerMessage message, CancellationToken cancellationToken)
    {
        var manufacturer = await _dbContext.Manufacturers
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == message.Id
            && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Manufacturer with id: {message.Id} is not found!");

        var existInSku = _dbContext.Skus.Any(a => a.ManufacturerId == message.Id);

        if (!existInSku)
        {
            manufacturer.IsDeleted = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Производитель используется в других таблицах");
    }
}
