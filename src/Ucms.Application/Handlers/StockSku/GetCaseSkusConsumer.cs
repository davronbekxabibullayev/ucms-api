namespace Ucms.Application.Handlers.StockSku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetCaseSkusMessage(
    PagedRequest Paging,
    Guid OrganizationId,
    Guid StockId,
    string? Seria) : IRequest<PagedResult<StockSkuModel>>;

public class GetCaseSkusConsumer : RequestHandler<GetCaseSkusMessage, PagedResult<StockSkuModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCaseSkusConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<StockSkuModel>> Handle(GetCaseSkusMessage message,
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
            .ToPagedResultAsync<StockSku, StockSkuModel>(message.Paging, _mapper, cancellationToken);
    }
}
