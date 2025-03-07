namespace Ucms.Stock.Api.Application.Consumers.Stats;

using System.Threading;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Core.Services;
using System.Threading.Tasks;
using Ucms.Organization.Clients;
using Ucms.Core.Services.Mediator;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Infrastructure.Persistance;
using Ucms.Common.Contracts.Models.Dashboards;

public record GetAmbulanceStockStatsMessage(
    Guid? OrganizationId,
    Guid? RegionId,
    Guid? CityId) : IRequest<DashboardWidgetModel>;

public class GetAmbulanceStocksStatsConsumer : RequestHandler<GetAmbulanceStockStatsMessage, DashboardWidgetModel>
{
    private readonly IWorkContext _workContext;
    private readonly IStockDbContext _stockDbContext;
    private readonly IOrganizationClient _organizationClient;

    public GetAmbulanceStocksStatsConsumer(
        IWorkContext workContext,
        IStockDbContext stockDbContext,
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

    private IQueryable<Domain.Models.StockSku> GetStockSkues()
    {
        return _stockDbContext.StockSkus
            .Where(x => x.Sku!.EmergencyType == EmergencyServiceType.Ambulance);
    }
}
