namespace Ucms.Application.Handlers.Stats;

using System.Threading;
using Ucms.Domain.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Dashboards;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;
using Ucms.Application.Abstractions.Organization;

public record GetAmbulanceStockStatsMessage(
    Guid? OrganizationId,
    Guid? RegionId,
    Guid? CityId) : IRequest<DashboardWidgetModel>;

public class GetAmbulanceStocksStatsConsumer : RequestHandler<GetAmbulanceStockStatsMessage, DashboardWidgetModel>
{
    private readonly IWorkContext _workContext;
    private readonly IUcmsDbContext _stockDbContext;
    private readonly IOrganizationClient _organizationClient;

    public GetAmbulanceStocksStatsConsumer(
        IWorkContext workContext,
        IUcmsDbContext stockDbContext,
        IOrganizationClient organizationClient)
    {
        _workContext = workContext;
        _stockDbContext = stockDbContext;
        _organizationClient = organizationClient;
    }

    protected override async Task<DashboardWidgetModel> Handle(GetAmbulanceStockStatsMessage message, CancellationToken cancellationToken)
    {
        var organizationId = message.OrganizationId ?? _workContext.TenantId;
        var organizationIds = await _organizationClient.GetOrganizationIds(organizationId, true, message.RegionId, message.CityId);
        var allOrganizationIds = organizationIds.Append(organizationId);

        var stockSkues = GetStockSkues();

        var stockCategoryAmount = await stockSkues.Where(w => allOrganizationIds.Contains(w.Stock!.OrganizationId) && w.Stock.StockType != StockType.Case)
            .GroupBy(g => g.Stock!.StockCategory)
            .Select(s => new
            {
                Category = s.Key,
                Amount = s.Sum(s => s.Amount)
            })
            .ToDictionaryAsync(d => d.Category, d => d.Amount, cancellationToken);

        var caseAmount = await stockSkues
            .Where(w => allOrganizationIds.Contains(w.Stock!.OrganizationId) && w.Stock.StockType == StockType.Case)
            .SumAsync(x => x.Amount, cancellationToken);

        var stockSkuWidget = new DashboardWidgetModel
        {
            Title = "MAVJUD DORI VOSITALARI",
            TitleRu = "ДОСТУПНЫЕ ЛЕКАРСТВА",
            TitleEn = "AVAILABLE MEDICINES",
            TitleKa = "MAVJUD DORI VOSITALARI",
            Items =
            [
                new()
                {
                    Title = "Asosiy omborlarda",
                    TitleRu = "На основных складах",
                    TitleEn = "In the main warehouses",
                    TitleKa = "Asosiy omborlarda",
                    Count = (int)stockCategoryAmount.GetValueOrDefault(StockCategory.Central)
                },
                new()
                {
                    Title = "Podstansiyalarda",
                    TitleRu = "На подстанциях",
                    TitleEn = "In substations",
                    TitleKa = "Podstansiyalarda",
                    Count = (int)stockCategoryAmount.GetValueOrDefault(StockCategory.Default)
                },
                new()
                {
                    Title = "Sumkalarda",
                    TitleRu = "В сумках",
                    TitleEn = "In cases",
                    TitleKa = "Sumkalarda",
                    Count = (int)caseAmount
                }
            ]
        };

        stockSkuWidget.TotalCount = stockSkuWidget.Items.Sum(x => x.Count);

        return stockSkuWidget;
    }

    private IQueryable<Domain.Entities.StockSku> GetStockSkues()
    {
        return _stockDbContext.StockSkus;
    }
}
