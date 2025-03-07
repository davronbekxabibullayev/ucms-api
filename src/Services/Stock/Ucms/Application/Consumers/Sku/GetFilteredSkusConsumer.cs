namespace Ucms.Stock.Api.Application.Consumers.Sku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Paging;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredSkusMessage(PagingRequest Paging, string? Query, string? SerialNumber) : IRequest<PagedList<SkuModel>>;

public class GetFilteredSkusConsumer : RequestHandler<GetFilteredSkusMessage, PagedList<SkuModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetFilteredSkusConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<PagedList<SkuModel>> Handle(GetFilteredSkusMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.OrganizationSkus
            .Include(i => i.Sku!.Manufacturer)
            .Include(i => i.Sku!.MeasurementUnit)
            .Include(i => i.Sku!.Supplier)
            .Where(w => w.Sku!.EmergencyType == _workContext.EmergencyType && w.OrganizationId == _workContext.TenantId)
            .Select(s => s.Sku!);

        if (!string.IsNullOrEmpty(message.Query))
        {
            var searchQuery = message.Query.ToLowerInvariant().Trim();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchQuery)
                || x.NameRu.ToLower().Contains(searchQuery)
                || x.NameKa!.ToLower().Contains(searchQuery)
                || x.NameEn!.ToLower().Contains(searchQuery));
        }

        if (!string.IsNullOrEmpty(message.SerialNumber))
        {
            var searialNumber = message.SerialNumber.ToLowerInvariant().Trim();
            query = query.Where(w => w.SerialNumber.ToLower().Contains(searialNumber));
        }

        return await query.ToPagedListAsync<Sku, SkuModel>(message.Paging, _mapper);
    }
}
