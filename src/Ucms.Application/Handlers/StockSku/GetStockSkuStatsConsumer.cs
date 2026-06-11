namespace Ucms.Application.Handlers.StockSku;

using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;
using Ucms.Application.Abstractions.Mediator;
using Ucms.Application.Abstractions.Organization;

public record GetStockSkuStatsMessage(Guid OrganizationId) : IRequest<StockSkuStatModel>;

public class GetStockSkuStatsConsumer : RequestHandler<GetStockSkuStatsMessage, StockSkuStatModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IOrganizationClient _organizationClient;

    public GetStockSkuStatsConsumer(IUcmsDbContext dbContext, IOrganizationClient organizationClient)
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
            .Where(w => w.Stock!.StockType == StockType.Car)
            .Sum(s => s.Amount);

        var caseStockSkusAmount = stockSkusAmount
            .Where(w => w.Stock!.StockType == StockType.Case)
            .Sum(s => s.Amount);

        var othersStockSkusAmount = stockSkusAmount
            .Where(w => w.Stock!.StockType != StockType.Car && w.Stock!.StockType != StockType.Case)
            .Sum(s => s.Amount);

        return new StockSkuStatModel(carStockSkusAmount, caseStockSkusAmount, othersStockSkusAmount);
    }
}
