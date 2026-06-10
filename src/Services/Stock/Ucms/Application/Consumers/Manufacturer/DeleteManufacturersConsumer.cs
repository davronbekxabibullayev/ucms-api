namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record DeleteManufacturersMessage(Guid[] Ids) : IRequest<bool>;

public class DeleteManufacturersConsumer : RequestHandler<DeleteManufacturersMessage, bool>
{
    private readonly IStockDbContext _dbContext;

    public DeleteManufacturersConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<bool> Handle(DeleteManufacturersMessage message, CancellationToken cancellationToken)
    {
        var manufacturers = await _dbContext.Manufacturers
            .AsTracking()
            .Where(x => message.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var existInSku = _dbContext.Skus.Any(a => message.Ids.Contains(a.ManufacturerId ?? Guid.Empty));

        if (!existInSku)
        {
            if (manufacturers.Count > 0)
                foreach (var manufacturer in manufacturers)
                    manufacturer.IsDeleted = true;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
        else
            throw new AppException("Производитель используется в других таблицах");
    }
}
