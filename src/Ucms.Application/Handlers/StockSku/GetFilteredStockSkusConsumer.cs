namespace Ucms.Application.Handlers.StockSku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetFilteredStockSkusMessage(PagedRequest Paging) : IRequest<PagedResult<StockSkuModel>>;

public class GetFilteredStockSkusConsumer : RequestHandler<GetFilteredStockSkusMessage, PagedResult<StockSkuModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFilteredStockSkusConsumer(IUcmsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<StockSkuModel>> Handle(GetFilteredStockSkusMessage message,
        CancellationToken cancellationToken)
    {
        return await _dbContext.StockSkus
            .Include(i => i.Stock)
            .Include(i => i.Sku)
            .OrderBy(a => a.Stock!.Name)
            .ToPagedResultAsync<StockSku, StockSkuModel>(message.Paging, _mapper, cancellationToken);
    }
}
