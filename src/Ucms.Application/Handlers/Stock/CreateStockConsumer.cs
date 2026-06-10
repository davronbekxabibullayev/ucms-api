namespace Ucms.Application.Handlers.Stock;

using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record CreateStockMessage(
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string Code,
    StorageCondition StorageCondition,
    StockType StockType,
    StockCategory StockCategory,
    Guid OrganizationId,
    Guid? ParentId,
    Guid[] EmployeeIds) : IRequest<Guid>;

public class CreateStockConsumer : RequestHandler<CreateStockMessage, Guid>
{
    private readonly IAppDbContext _dbContext;

    public CreateStockConsumer(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task<Guid> Handle(CreateStockMessage message, CancellationToken cancellationToken)
    {
        await ValidateOrThrowAsync(message, cancellationToken);

        var stock = MapToEntity(message);

        _dbContext.Stocks.Add(stock);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return stock.Id;
    }

    private async Task ValidateOrThrowAsync(CreateStockMessage message, CancellationToken cancellationToken)
    {
        var exist = await _dbContext.Stocks
            .AnyAsync(f => f.Code == message.Code, cancellationToken);
        if (exist)
            throw new AlreadyExistException(nameof(Stock), message.Code);

        if (message.StockCategory == StockCategory.Central)
        {
            exist = await _dbContext.Stocks
                .AnyAsync(f => f.StockCategory == StockCategory.Central
                            && f.OrganizationId == message.OrganizationId, cancellationToken);
            if (exist)
                throw new AlreadyExistException($"Central stock for this organization already exist!");
        }
    }

    private static Stock MapToEntity(CreateStockMessage message)
    {
        return new Stock
        {
            Name = message.Name,
            NameEn = message.NameEn,
            NameKa = message.NameKa,
            NameRu = message.NameRu,
            Code = message.Code,
            StorageCondition = message.StorageCondition,
            StockType = message.StockType,
            StockCategory = message.StockCategory,
            ParentId = message.ParentId,
            OrganizationId = message.OrganizationId,
            EmployeeIds = message.EmployeeIds
        };
    }
}
