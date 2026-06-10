namespace Ucms.Application.Handlers.Manufacturer;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record UpdateManufacturerMessage(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string? Code) : IRequest<Guid>;

public class UpdateManufacturerConsumer : RequestHandler<UpdateManufacturerMessage, Guid>
{
    private readonly IAppDbContext _dbContext;

    public UpdateManufacturerConsumer(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<Guid> Handle(UpdateManufacturerMessage message, CancellationToken cancellationToken)
    {
        var manufacturer = await _dbContext.Manufacturers
                .FirstOrDefaultAsync(x => x.Id == message.Id, cancellationToken)
                ?? throw new NotFoundException($"Manufacturer with Id : {message.Id} is not found!");

        manufacturer.NameEn = message.NameEn;
        manufacturer.NameRu = message.NameRu;
        manufacturer.NameKa = message.NameKa;
        manufacturer.Name = message.Name;
        manufacturer.Code = message.Code;

        _dbContext.Manufacturers.Update(manufacturer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return message.Id;
    }
}
