namespace Ucms.Stock.Api.Application.Consumers.Stock;

using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Organization.Clients;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record DeleteStockMessage(Guid Id) : IRequest<bool>;

public class DeleteStockConsumer : RequestHandler<DeleteStockMessage, bool>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IOrganizationClient _organizationClient;

    public DeleteStockConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext,
        IOrganizationClient organizationClient)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _organizationClient = organizationClient;
    }

    protected override async Task<bool> Handle(DeleteStockMessage message, CancellationToken cancellationToken)
    {
        var stock = await GetStockOrThrowAsync(message.Id);

        var existInOrganizationBrigade = await _organizationClient.CheckOrganizationBrigadeStock(stock.Id);
        if (existInOrganizationBrigade)
            throw new AppException("Сумка прикреплена к бригаде!");

        var existInIncome = _dbContext.Incomes.Any(a => a.StockId == message.Id);
        var existInOutcome = _dbContext.Outcomes.Any(a => a.StockId == message.Id);
        var existInBalance = _dbContext.StockSkus.Any(a => a.StockId == message.Id);
        var existInParent = _dbContext.Stocks.Any(a => a.ParentId == message.Id);

        if (!existInIncome && !existInOutcome && !existInBalance && !existInParent)
        {
            stock.IsDeleted = true;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        else
            throw new AppException("Склад используется в других таблицах");
    }

    private async Task<Stock> GetStockOrThrowAsync(Guid id)
    {
        if (_workContext.IsAdmin)
            return await _dbContext.Stocks
                .AsTracking()
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new NotFoundException();

        return await _dbContext.Stocks
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.OrganizationId == _workContext.TenantId)
            ?? throw new NotFoundException();
    }
}
