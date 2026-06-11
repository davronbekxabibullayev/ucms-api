namespace Ucms.Application.Handlers.Stock;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetAllCasesMessage() : IRequest<List<StockModel>>;

public class GetAllCasesConsumer : RequestHandler<GetAllCasesMessage, List<StockModel>>
{
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IUcmsDbContext _dbContext;

    public GetAllCasesConsumer(
        IMapper mapper,
        IWorkContext workContext,
        IUcmsDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _workContext = workContext;
    }
    protected override async Task<List<StockModel>> Handle(GetAllCasesMessage message, CancellationToken cancellationToken)
    {
        var stocks = await _dbContext.Stocks
            .Where(w => w.OrganizationId == _workContext.TenantId && w.StockType == StockType.Case)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<StockModel>>(stocks);
    }
}
