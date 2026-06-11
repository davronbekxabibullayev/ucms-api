namespace Ucms.Application.Handlers.Manufacturer;

using System.Threading;
using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetStockSkuManufacturersMessage(string? Query, Guid? OrganizationId, Guid? StockId, Guid? ProductId, PagedRequest Request) : IRequest<PagedResult<ManufacturerModel>>;

public class GetStockSkuManufacturersConsumer : RequestHandler<GetStockSkuManufacturersMessage, PagedResult<ManufacturerModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetStockSkuManufacturersConsumer(IUcmsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<PagedResult<ManufacturerModel>> Handle(GetStockSkuManufacturersMessage message, CancellationToken cancellationToken)
    {
        var stockSku = _dbContext.StockSkus
            .AsQueryable();

        if (message.OrganizationId != null)
        {
            stockSku = stockSku.Where(w => w.Stock!.OrganizationId == message.OrganizationId);
        }

        if (message.StockId != null)
        {
            stockSku = stockSku.Where(w => w.StockId == message.StockId);
        }

        if (message.ProductId != null)
        {
            stockSku = stockSku.Where(w => w.Sku!.ProductId == message.ProductId);
        }

        var manufacturers = stockSku.Select(s => s.Sku!.Manufacturer!).Distinct();

        if (!string.IsNullOrEmpty(message.Query))
        {
            var query = message.Query.ToLowerInvariant().Trim();
            manufacturers = manufacturers.Where(x =>
                x.Name.ToLower().Contains(query) ||
                x.NameRu.ToLower().Contains(query) ||
                x.NameKa!.ToLower().Contains(query) ||
                x.NameEn!.ToLower().Contains(query) ||
                x.Code!.ToLower().Contains(query));
        }

        return await manufacturers.ToPagedResultAsync<Manufacturer, ManufacturerModel>(message.Request, _mapper, cancellationToken);
    }
}
