namespace Ucms.Application.Handlers.Sku;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetFilteredSkusMessage(PagedRequest Paging, string? Query, string? SerialNumber) : IRequest<PagedResult<SkuModel>>;

public class GetFilteredSkusConsumer : RequestHandler<GetFilteredSkusMessage, PagedResult<SkuModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetFilteredSkusConsumer(IAppDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<PagedResult<SkuModel>> Handle(GetFilteredSkusMessage message,
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

        return await query.ToPagedResultAsync<Sku, SkuModel>(message.Paging, _mapper, cancellationToken);
    }
}
