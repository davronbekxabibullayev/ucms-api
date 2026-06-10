namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using System.Threading;
using AutoMapper;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetStockSkuManufacturersMessage(string? Query, Guid? OrganizationId, Guid? StockId, Guid? ProductId, PagingRequest Request) : IRequest<PagedList<ManufacturerModel>>;

public class GetStockSkuManufacturersConsumer : RequestHandler<GetStockSkuManufacturersMessage, PagedList<ManufacturerModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetStockSkuManufacturersConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<PagedList<ManufacturerModel>> Handle(GetStockSkuManufacturersMessage message, CancellationToken cancellationToken)
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

        return await manufacturers.ToPagedListAsync<Manufacturer, ManufacturerModel>(message.Request, _mapper);
    }
}
