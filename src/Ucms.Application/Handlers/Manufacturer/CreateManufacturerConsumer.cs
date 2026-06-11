namespace Ucms.Application.Handlers.Manufacturer;

using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record CreateManufacturerMessage(string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string? Code) : IRequest<Guid>;

public class CreateManufacturerConsumer : RequestHandler<CreateManufacturerMessage, Guid>
{
    private readonly IUcmsDbContext _dbContext;

    public CreateManufacturerConsumer(IUcmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<Guid> Handle(CreateManufacturerMessage message, CancellationToken cancellationToken)
    {
        var manufacturer = await _dbContext.Manufacturers
            .FirstOrDefaultAsync(x => x.Code == message.Code, cancellationToken);

        if (manufacturer != null)
        {
            throw new AlreadyExistException($"Manufacturer with code: {message.Code} , already exist!");
        }

        manufacturer = new Manufacturer
        {
            Name = message.Name,
            NameEn = message.NameEn,
            NameRu = message.NameRu,
            NameKa = message.NameKa,
            Code = message.Code
        };

        await _dbContext.Manufacturers.AddAsync(manufacturer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return manufacturer.Id;
    }
}
