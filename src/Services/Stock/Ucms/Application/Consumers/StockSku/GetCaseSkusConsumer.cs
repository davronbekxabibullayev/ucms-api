namespace Ucms.Stock.Api.Application.Consumers.StockSku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Models.Enums;
using Ucms.Common.Paging;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetCaseSkusMessage(
    PagingRequest Paging,
    Guid OrganizationId,
    Guid StockId,
    string? Seria) : IRequest<PagedList<StockSkuModel>>;

public class GetCaseSkusConsumer : RequestHandler<GetCaseSkusMessage, PagedList<StockSkuModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCaseSkusConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedList<StockSkuModel>> Handle(GetCaseSkusMessage message,
        CancellationToken cancellationToken)
    {
        if (!_dbContext.Stocks.Any(a => a.Id == message.StockId && a.StockType == StockType.Case))
            throw new AppException("Stock type is not a case!");

        var query = _dbContext.StockSkus
            .Include(i => i.Sku!.Product)
            .Include(i => i.Stock)
            .Include(i => i.MeasurementUnit)
            .Where(w => w.Stock!.OrganizationId == message.OrganizationId
                     && w.StockId == message.StockId);

        if (!string.IsNullOrEmpty(message.Seria))
        {
            var seria = message.Seria.ToLower();
            query = query.Where(w => w.Sku!.SerialNumber.ToLower().Contains(seria));
        }

        return await query
            .OrderBy(a => a.Stock!.Name)
            .ToPagedListAsync<StockSku, StockSkuModel>(message.Paging, _mapper);
    }
}
