namespace Ucms.Application.Handlers.Sku;

using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record FindSkusMessage(string Query) : IRequest<List<SkuModel>>;

public class FindSkusConsumer : RequestHandler<FindSkusMessage, List<SkuModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public FindSkusConsumer(IAppDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<List<SkuModel>> Handle(FindSkusMessage message, CancellationToken cancellationToken)
    {
        var query = message.Query.ToLower();

        var skus = await _dbContext.Skus
            .Where(a =>
                a.EmergencyType == _workContext.EmergencyType &&
                (a.Name.ToLower().Contains(query) ||
                 a.NameEn!.ToLower().Contains(query) ||
                 a.NameRu.ToLower().Contains(query) ||
                 a.NameKa!.ToLower().Contains(query) ||
                 a.SerialNumber.Contains(query)))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<SkuModel>>(skus);

        return result;
    }
}
