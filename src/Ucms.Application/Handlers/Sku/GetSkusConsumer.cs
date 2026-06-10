namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetSkusMessage : IRequest<List<SkuModel>>;

public class GetSkusConsumer : RequestHandler<GetSkusMessage, List<SkuModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetSkusConsumer(IAppDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<List<SkuModel>> Handle(GetSkusMessage message, CancellationToken cancellationToken)
    {
        var skus = await _dbContext.OrganizationSkus
            .Where(w => w.Sku!.EmergencyType == _workContext.EmergencyType && w.OrganizationId == _workContext.TenantId)
            .OrderBy(a => a.Sku!.Name)
            .Select(s => s.Sku)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<SkuModel>>(skus);

        return result;
    }
}
