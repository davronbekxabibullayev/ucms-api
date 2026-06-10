namespace Ucms.Application.Handlers.StockSku;

using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetStockInventoryMessage(
    PagedRequest Paging,
    Guid? StockId,
    Guid? ProductId,
    Guid? OrganizationId) : IRequest<PagedResult<StockInventoryModel>>;

public class GetStockInventoryConsumer(
    IAppDbContext dbContext,
    IWorkContext workContext) : RequestHandler<GetStockInventoryMessage, PagedResult<StockInventoryModel>>
{

    protected override async Task<PagedResult<StockInventoryModel>> Handle(GetStockInventoryMessage message,
        CancellationToken cancellationToken)
    {
        var query = dbContext.StockSkus.AsQueryable();

        if (message.OrganizationId != null)
        {
            query = query.Where(w => w.Stock!.OrganizationId == message.OrganizationId);
        }

        if (message.StockId != null)
        {
            query = query.Where(w => w.StockId == message.StockId);
        }

        if (message.ProductId != null)
        {
            query = query.Where(w => w.Sku!.ProductId == message.ProductId);
        }

        var result = await query
            .OrderBy(o => o.Sku!.NameRu)
            .Select(s => new StockInventoryDataModel
            {
                Amount = s.Amount,
                ProductId = s.Sku!.ProductId,
                MeasurementUnitId = s.MeasurementUnitId,
                SkuName = s.Sku!.Name,
                SkuNameEn = s.Sku!.NameEn,
                SkuNameRu = s.Sku!.NameRu,
                SkuNameKa = s.Sku!.NameKa,
                MeasurementUnitName = s.MeasurementUnit!.Name,
                MeasurementUnitNameEn = s.MeasurementUnit!.NameEn,
                MeasurementUnitNameRu = s.MeasurementUnit!.NameRu,
                MeasurementUnitNameKa = s.MeasurementUnit!.NameKa,
                MeasurementUnitType = s.MeasurementUnit!.Type
            })
            .GroupBy(a => new { a.ProductId, a.MeasurementUnitId })
            .Select(s => new StockInventoryModel { Data = s.First(), Amount = s.Sum(ss => ss.Amount) })
            .ToPagedResultAsync(message.Paging, cancellationToken);
        var muTypes = result.Items.Select(s => s.Data!.MeasurementUnitType);
        var measurementUnits = await dbContext.OrganizationMeasurementUnits
            .Where(w => w.OrganizationId == workContext.TenantId && muTypes.Contains(w.Type))
            .Select(s => s.MeasurementUnit)
            .ToListAsync(cancellationToken);
        result.Items.ForEach(item =>
        {
            if (item.Data != null)
            {
                var measurementUnit = measurementUnits.FirstOrDefault(f => f!.Type == item.Data.MeasurementUnitType);
                if (measurementUnit != null)
                {
                    item.Data.MeasurementUnitId = measurementUnit.Id;
                    item.Data.MeasurementUnitName = measurementUnit.Name;
                    item.Data.MeasurementUnitNameEn = measurementUnit.NameEn;
                    item.Data.MeasurementUnitNameRu = measurementUnit.NameRu;
                    item.Data.MeasurementUnitNameKa = measurementUnit.NameKa;
                    item.Data.MeasurementUnitType = measurementUnit.Type;
                    item.Amount /= measurementUnit.Multiplier > 0 ? measurementUnit.Multiplier : 1;
                }
            }
        });

        return result;
    }
}
