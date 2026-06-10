namespace Ucms.Application.Handlers.StockSku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Authorization;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;
using Ucms.Application.Abstractions.Constants;

public record GetStockSkusMessage(
    Guid OrganizationId,
    PagedRequest Paging,
    Guid? StockId,
    Guid? MeasurementUnitId,
    Guid? ProductId,
    Guid? ManufacturerId,
    string? Seria) : IRequest<PagedResult<StockSkuModel>>;

public class GetStockSkusConsumer : RequestHandler<GetStockSkusMessage, PagedResult<StockSkuModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IPermissionProvider _permissionProvider;

    public GetStockSkusConsumer(
        IAppDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext,
        IPermissionProvider permissionProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedResult<StockSkuModel>> Handle(GetStockSkusMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StockSkus
            .Include(i => i.Sku!.Manufacturer)
            .Include(i => i.Sku!.Product)
            .Include(i => i.Stock)
            .Include(i => i.MeasurementUnit)
            .Where(s => s.Stock!.OrganizationId == message.OrganizationId);

        if (_workContext.TenantId == message.OrganizationId)
        {
            if (!await _permissionProvider.HasPermissionAsync(Permissions.Warehouse.AccessSettingMinimumBalanceWarehouse, cancellationToken))
            {
                query = query.Where(w => w.Stock!.EmployeeIds.Contains(_workContext.EmployeeId ?? Guid.Empty));
            }
        }

        if (message.StockId != null)
            query = query.Where(w => w.StockId == message.StockId);

        if (message.ProductId != null)
            query = query.Where(w => w.Sku!.ProductId == message.ProductId);

        if (message.ManufacturerId != null)
            query = query.Where(w => w.Sku!.ManufacturerId == message.ManufacturerId);

        if (!string.IsNullOrEmpty(message.Seria))
        {
            var seria = message.Seria.ToLower();
            query = query.Where(w => w.Sku!.SerialNumber.ToLower().Contains(seria));
        }

        var result = await query
            .OrderBy(a => a.Stock!.Name)
            .ToPagedResultAsync<StockSku, StockSkuModel>(message.Paging, _mapper, cancellationToken);

        await ChangeMeasurementUnit(result, cancellationToken);

        return result;
    }

    private async Task ChangeMeasurementUnit(PagedResult<StockSkuModel> result, CancellationToken cancellationToken)
    {
        var measurementUnitTypes = result.Items
                    .Where(w => w.MeasurementUnit != null)
                    .Select(s => s.MeasurementUnit!.Type)
                    .ToList();
        var measurementUnits = await _dbContext.OrganizationMeasurementUnits
            .Where(f => f.OrganizationId == _workContext.TenantId && measurementUnitTypes.Contains(f.Type))
            .Select(s => s.MeasurementUnit)
            .ToListAsync(cancellationToken);
        result.Items.ForEach(item =>
        {
            if (item.MeasurementUnit != null)
            {
                var measurementUnit = measurementUnits.FirstOrDefault(f => f!.Type == item.MeasurementUnit!.Type);
                if (measurementUnit != null)
                {
                    var measurementUnitModel = _mapper.Map<MeasurementUnitModel>(measurementUnit);
                    item.MeasurementUnit = measurementUnitModel;
                    item.Amount /= measurementUnitModel.Multiplier > 0 ? measurementUnitModel.Multiplier : 1;
                }
            }
        });
    }
}
