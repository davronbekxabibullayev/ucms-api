namespace Ucms.Stock.Api.Application.Consumers.StockSku;

using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services.Mediator;
using Ucms.Organization.Clients;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetStockSkuStatsMessage(Guid OrganizationId) : IRequest<StockSkuStatModel>;

public class GetStockSkuStatsConsumer : RequestHandler<GetStockSkuStatsMessage, StockSkuStatModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IOrganizationClient _organizationClient;

    public GetStockSkuStatsConsumer(IStockDbContext dbContext, IOrganizationClient organizationClient)
    {
        _dbContext = dbContext;
        _organizationClient = organizationClient;
    }

    protected override async Task<StockSkuStatModel> Handle(GetStockSkuStatsMessage message,
        CancellationToken cancellationToken)
    {
        var organizationIds = await _organizationClient.GetOrganizationIds(message.OrganizationId);

        var stockSkusAmount = _dbContext.StockSkus.Where(w => organizationIds.Contains(w.Stock!.OrganizationId));

        var carStockSkusAmount = stockSkusAmount
            .Where(w => w.Stock!.StockType == Common.Enums.StockType.Car)
            .Sum(s => s.Amount);

        var caseStockSkusAmount = stockSkusAmount
            .Where(w => w.Stock!.StockType == Common.Enums.StockType.Case)
            .Sum(s => s.Amount);

        var othersStockSkusAmount = stockSkusAmount
            .Where(w => w.Stock!.StockType != Common.Enums.StockType.Car && w.Stock!.StockType != Common.Enums.StockType.Case)
            .Sum(s => s.Amount);

        return new StockSkuStatModel(carStockSkusAmount, caseStockSkusAmount, othersStockSkusAmount);
    }
}
