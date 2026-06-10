namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetProductStockSkusMessage(
    PagedRequest Paging,
    Guid? ProductId,
    Guid? StockId,
    List<ProductType>? Types,
    string? Query) : IRequest<List<SkuModel>>;

public class GetProductStockSkusConsumer : RequestHandler<GetProductStockSkusMessage, List<SkuModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetProductStockSkusConsumer(
        IAppDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<List<SkuModel>> Handle(GetProductStockSkusMessage message, CancellationToken cancellationToken)
    {
        var query = _dbContext.StockSkus
            .Include(i => i.Sku!.Product)
            .Include(i => i.Sku!.MeasurementUnit)
            .Where(w => w.Amount > 0 && w.Stock!.OrganizationId == _workContext.TenantId);

        if (message.ProductId != null)
        {
            query = query.Where(w => w.Sku!.ProductId == message.ProductId);
        }

        if (message.StockId != null)
        {
            query = query.Where(w => w.StockId == message.StockId);
        }

        if (message.Types != null)
        {
            query = query.Where(w => message.Types.Contains(w.Sku!.Product!.Type));
        }

        if (!string.IsNullOrEmpty(message.Query))
        {
            var name = message.Query!.ToLower();
            query = query.Where(w => w.Sku!.Name.ToLower().Contains(name) ||
                 w.Sku!.NameEn!.ToLower().Contains(name) ||
                 w.Sku!.NameRu.ToLower().Contains(name) ||
                 w.Sku!.NameKa!.ToLower().Contains(name) ||
                 w.Sku!.SerialNumber.Contains(name));
        }

        query = query.Skip(message.Paging.Page).Take(message.Paging.PageSize);

        return await MapToSku(query, cancellationToken);
    }

    private async Task<List<SkuModel>> MapToSku(IQueryable<StockSku> query, CancellationToken cancellationToken)
    {
        var stockSkus = await query
            .OrderBy(o => o.Sku!.Name)
            .ToListAsync(cancellationToken);

        return stockSkus.Select(s =>
        {
            var sku = _mapper.Map<SkuModel>(s.Sku);
            sku.StockSkuAmount = s.Amount;
            return sku;
        }).ToList();
    }
}
