namespace Ucms.Application.Handlers.Sku;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetProductSkusMessage(Guid ProductId) : IRequest<List<SkuModel>>;

public class GetProductSkusConsumer : RequestHandler<GetProductSkusMessage, List<SkuModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetProductSkusConsumer(IUcmsDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<List<SkuModel>> Handle(GetProductSkusMessage message, CancellationToken cancellationToken)
    {
        var skus = await _dbContext.OrganizationSkus
            .Include(i => i.Sku!.MeasurementUnit)
            .Where(w => w.OrganizationId == _workContext.TenantId
                     && w.Sku!.ProductId == message.ProductId)
            .Select(w => w.Sku!)
            .OrderBy(a => a!.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<SkuModel>>(skus);

        return result;
    }
}
