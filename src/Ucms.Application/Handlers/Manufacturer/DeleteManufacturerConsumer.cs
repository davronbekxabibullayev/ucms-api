namespace Ucms.Application.Handlers.Manufacturer;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record DeleteManufacturerMessage(Guid Id) : IRequest<bool>;
public class DeleteManufacturerConsumer : RequestHandler<DeleteManufacturerMessage, bool>
{
    private readonly IAppDbContext _dbContext;

    public DeleteManufacturerConsumer(IAppDbContext dbContext)
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
