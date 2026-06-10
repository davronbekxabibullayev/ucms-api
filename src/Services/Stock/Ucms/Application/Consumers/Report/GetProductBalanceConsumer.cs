namespace Ucms.Stock.Api.Application.Consumers.Report;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetProductBalanceReportMessage(
    DateTime From,
    DateTime To,
    Guid OrganizationId
) : IRequest<ProductBalanceReportModel>;

public class GetProductBalanceReportConsumer : RequestHandler<GetProductBalanceReportMessage, ProductBalanceReportModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;

    public GetProductBalanceReportConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext)
    {
        _dbContext = dbContext;
        _workContext = workContext;
    }

    protected override async Task<ProductBalanceReportModel> Handle(GetProductBalanceReportMessage message, CancellationToken cancellationToken)
    {
        var skuIds = await _dbContext.OrganizationSkus
                .Where(w => w.OrganizationId == message.OrganizationId)
                .Select(s => s.SkuId)
                .ToListAsync(cancellationToken);

        var registerRecords = await _dbContext.StockBalanceRegistry
            .Include(i => i.Sku)
            .Include(i => i.Product)
            .Include(i => i.MeasurementUnit)
            .Include(i => i.Stock)
            .Where(w => w.Date >= message.From
                     && w.Date <= message.To
                     && skuIds.Contains(w.SkuId))
            .OrderByDescending(w => w.Date)
            .ToListAsync(cancellationToken);

        var result = GetProductBalanceReportModel(message, registerRecords);

        await UpdateMeasurementUnits(result, cancellationToken);

        return result;
    }

    private ProductBalanceReportModel GetProductBalanceReportModel(GetProductBalanceReportMessage message, List<StockBalanceRegister> registerRecords)
    {
        return new ProductBalanceReportModel
        {
            From = message.From,
            To = message.To,
            OrganizationId = message.OrganizationId,
            ProductTypes = registerRecords.GroupBy(g => g.Product!.Type).Select(s => new ProductBalanceReportProductTypeModel
            {
                ProductType = s.Key,
                Products = s.GroupBy(g => g.ProductId).Select(ss => new ProductBalanceReportProductModel
                {
                    ProductName = ss.FirstOrDefault()!.Product!.Name,
                    ProductNameRu = ss.FirstOrDefault()!.Product!.NameRu,
                    ProductNameEn = ss.FirstOrDefault()!.Product!.NameEn,
                    ProductNameKa = ss.FirstOrDefault()!.Product!.NameKa,
                    MeasurementUnitName = ss.FirstOrDefault()!.MeasurementUnit!.Name,
                    MeasurementUnitNameRu = ss.FirstOrDefault()!.MeasurementUnit!.NameRu,
                    MeasurementUnitNameEn = ss.FirstOrDefault()!.MeasurementUnit!.NameEn,
                    MeasurementUnitNameKa = ss.FirstOrDefault()!.MeasurementUnit!.NameKa,
                    MeasurementUnitType = ss.FirstOrDefault()!.MeasurementUnit!.Type,
                    Skus = ss.GroupBy(g => g.Sku!.SerialNumber).Select(sss => new ProductBalanceReportSkuModel
                    {
                        Seria = sss.Key,
                        ExpirationDate = sss.FirstOrDefault()!.Sku!.ExpirationDate,

                        CentralStockFromBalance = GetStockFromBalance(sss, StockCategory.Central, message.From),
                        ChildStocksFromBalance = GetStockFromBalance(sss, StockCategory.Default, message.From),

                        CentralStockIncome = sss.Where(w => w.Stock!.StockCategory == StockCategory.Central
                                                         && w.VariableAmount > 0)
                                                .Sum(sum => sum.VariableAmount),
                        CentralStockBroadcastOutcome = sss.Where(w => w.Stock!.StockCategory == StockCategory.Central
                                                                   && w.VariableAmount < 0)
                                                          .Sum(sum => Math.Abs(sum.VariableAmount)),
                        AllStocksUsageOutcome = sss.Where(w => w.VariableAmount < 0
                                                            && w.Type == (int)OutcomeType.Usage)
                                                   .Sum(sum => Math.Abs(sum.VariableAmount)),

                        CentralStockToBalance = GetStockToBalance(sss, StockCategory.Central),
                        ChildStocksToBalance = GetStockToBalance(sss, StockCategory.Default),
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }

    private decimal GetStockFromBalance(IGrouping<string, StockBalanceRegister> stockBalances, StockCategory category, DateTime from)
    {
        var fromBalance = stockBalances.LastOrDefault(l => l.Stock!.StockCategory == category);

        if (fromBalance == null)
            return 0;

        if (fromBalance.Date.Date == from.Date)
            return fromBalance.CurrentAmount;
        else
            return fromBalance.PreviousAmount;
    }

    private decimal GetStockToBalance(IGrouping<string, StockBalanceRegister> stockBalances, StockCategory category)
    {
        var toBalance = stockBalances.FirstOrDefault(l => l.Stock!.StockCategory == category);

        if (toBalance == null)
            return 0;

        return toBalance.CurrentAmount;
    }

    private async Task UpdateMeasurementUnits(ProductBalanceReportModel report, CancellationToken cancellationToken)
    {
        var mus = await _dbContext.OrganizationMeasurementUnits
            .Where(w => w.OrganizationId == _workContext.TenantId)
            .Select(s => s.MeasurementUnit)
            .ToListAsync(cancellationToken);
        report.ProductTypes.ForEach(productType =>
        {
            productType.Products.ForEach(product =>
            {
                var mu = mus.FirstOrDefault(f => f!.Type == product.MeasurementUnitType);
                if (mu != null)
                {
                    var multiplier = mu.Multiplier > 0 ? mu.Multiplier : 1;
                    product.MeasurementUnitName = mu.Name;
                    product.MeasurementUnitNameRu = mu.NameRu;
                    product.MeasurementUnitNameEn = mu.NameEn;
                    product.MeasurementUnitNameKa = mu.NameKa;
                    product.Skus.ForEach(sku =>
                    {
                        sku.CentralStockFromBalance /= multiplier;
                        sku.ChildStocksFromBalance /= multiplier;

                        sku.CentralStockIncome /= multiplier;
                        sku.CentralStockBroadcastOutcome /= multiplier;
                        sku.AllStocksUsageOutcome /= multiplier;

                        sku.CentralStockToBalance /= multiplier;
                        sku.ChildStocksToBalance /= multiplier;
                    });
                }
            });
        });
    }
}
