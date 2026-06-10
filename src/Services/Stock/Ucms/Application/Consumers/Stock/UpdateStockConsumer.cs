namespace Ucms.Stock.Api.Application.Consumers.Stock;

using System.Threading;
using Common.Enums;
using Core.Exceptions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;

public record UpdateStockMessage(
    Guid Id,
    string Name,
    string NameRu,
    string? NameEn,
    string? NameKa,
    string? Code,
    StorageCondition StorageCondition,
    StockType StockType,
    StockCategory StockCategory,
    Guid OrganizationId,
    Guid? ParentId,
    Guid[] EmployeeIds) : IRequest<Guid>;

public class UpdateStockConsumer : RequestHandler<UpdateStockMessage, Guid>
{
    private readonly IStockDbContext _dbContext;

    public UpdateStockConsumer(IStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    protected override async Task<Guid> Handle(UpdateStockMessage message, CancellationToken cancellationToken)
    {
        await ValidateOrThrowAsync(message);

        var stock = await GetEntityOrThrowAsync(message);

        if (message.StockCategory == StockCategory.Central)
        {
            var exist = await _dbContext.Stocks.AnyAsync(a => a.StockCategory == StockCategory.Central
                                                           && a.Id != message.Id
                                                           && a.OrganizationId == message.OrganizationId, cancellationToken);
            if (exist)
                throw new AlreadyExistException($"Central stock for this organization already exist!");
        }

        MapToEntity(stock, message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return stock.Id;
    }

    private async Task<Stock> GetEntityOrThrowAsync(UpdateStockMessage message)
    {
        return await _dbContext.Stocks
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Id == message.Id)
            ?? throw new NotFoundException(nameof(Stock), message.Id);
    }

    private async Task ValidateOrThrowAsync(UpdateStockMessage message)
    {
        var isExist = await _dbContext.Stocks.AnyAsync(a => a.Id != message.Id && a.Code == message.Code);

        if (isExist)
        {
            throw new AlreadyExistException(nameof(Stock), message.Code!);
        }
    }

    private static void MapToEntity(Stock stock, UpdateStockMessage message)
    {
        stock.Name = message.Name;
        stock.NameEn = message.NameEn;
        stock.NameKa = message.NameKa;
        stock.NameRu = message.NameRu;
        stock.StorageCondition = message.StorageCondition;
        stock.StockType = message.StockType;
        stock.StockCategory = message.StockCategory;
        stock.ParentId = message.ParentId;
        stock.OrganizationId = message.OrganizationId;
        stock.EmployeeIds = message.EmployeeIds;
    }
}
