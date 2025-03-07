namespace Ucms.Stock.Api.Application.Consumers.StockSku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Paging;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetStockInventoryMessage(
    FilteringRequest Paging,
    Guid? StockId,
    Guid? ProductId,
    Guid? OrganizationId) : IRequest<PagedList<StockInventoryModel>>;

public class GetStockInventoryConsumer : RequestHandler<GetStockInventoryMessage, PagedList<StockInventoryModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetStockInventoryConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
    }

    protected override async Task<PagedList<StockInventoryModel>> Handle(GetStockInventoryMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StockSkus.AsQueryable();

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
            .ToPagedListAsync(message.Paging);
        var muTypes = result.Data.Select(s => s.Data!.MeasurementUnitType);
        var measurementUnits = await _dbContext.OrganizationMeasurementUnits
            .Where(w => w.OrganizationId == _workContext.TenantId && muTypes.Contains(w.Type))
            .Select(s => s.MeasurementUnit)
            .ToListAsync(cancellationToken);
        result.Data.ForEach(item =>
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
