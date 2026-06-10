namespace Ucms.Stock.Api.Application.Consumers.Stock;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetAllCasesMessage() : IRequest<List<StockModel>>;

public class GetAllCasesConsumer : RequestHandler<GetAllCasesMessage, List<StockModel>>
{
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IStockDbContext _dbContext;

    public GetAllCasesConsumer(
        IMapper mapper,
        IWorkContext workContext,
        IStockDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _workContext = workContext;
    }
    protected override async Task<List<StockModel>> Handle(GetAllCasesMessage message, CancellationToken cancellationToken)
    {
        var stocks = await _dbContext.Stocks
            .Where(w => w.OrganizationId == _workContext.TenantId && w.StockType == Common.Enums.StockType.Case)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<StockModel>>(stocks);
    }
}
