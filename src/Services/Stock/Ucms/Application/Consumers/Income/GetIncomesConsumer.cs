namespace Ucms.Stock.Api.Application.Consumers.Income;

using System.Threading;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Infrastructure.Persistance;
public record GetIncomesMessage : IRequest<List<IncomeModel>>;

public class GetIncomesConsumer : RequestHandler<GetIncomesMessage, List<IncomeModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;

    public GetIncomesConsumer(IStockDbContext dbContext, IWorkContext workContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
    }
    protected override async Task<List<IncomeModel>> Handle(GetIncomesMessage message, CancellationToken cancellationToken)
    {
        var incomes = await GetIncomesAsync();

        return incomes;
    }

    private async Task<List<IncomeModel>> GetIncomesAsync()
    {
        return await _dbContext.Incomes
            .Include(i => i.IncomeItems)
            .Include(i => i.Stock)
            .Where(w => w.Stock!.OrganizationId == _workContext.TenantId)
            .OrderByDescending(a => a.IncomeDate)
            .ProjectTo<IncomeModel>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
