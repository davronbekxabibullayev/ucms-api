namespace Ucms.Stock.Api.Application.Consumers.Outcome;

using AutoMapper;
using Devhub.Authorization;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Paging;
using Ucms.Core.Constants;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredOutcomesMessage(
    PagingRequest Paging,
    Guid? StockId,
    string? Query,
    DateTime? From,
    DateTime? To) : IRequest<PagedList<OutcomeModel>>;

public class GetFilteredOutcomesConsumer : RequestHandler<GetFilteredOutcomesMessage, PagedList<OutcomeModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;
    private readonly IPermissionProvider _permissionProvider;

    public GetFilteredOutcomesConsumer(
        IStockDbContext dbContext,
        IWorkContext workContext,
        IMapper mapper,
        IPermissionProvider permissionProvider)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedList<OutcomeModel>> Handle(GetFilteredOutcomesMessage message,
        CancellationToken cancellationToken)
    {
        return await GetOutcomes(message, cancellationToken);
    }

    private async Task<PagedList<OutcomeModel>> GetOutcomes(GetFilteredOutcomesMessage message, CancellationToken cancellationToken)
    {
        var query = _dbContext.Outcomes
            .Include(i => i.Stock)
            .Include(i => i.IncomeOutcome!.IncomeStock)
            .Include(i => i.OutcomeItems)
            .ThenInclude(th => th.MeasurementUnit)
            .Include(i => i.OutcomeItems)
            .ThenInclude(th => th.Sku!.MeasurementUnit)
            .Where(w => w.Stock!.OrganizationId == _workContext.TenantId);

        if (!await _permissionProvider.HasPermissionAsync(Permissions.Warehouse.AccessSettingMinimumBalanceWarehouse, cancellationToken))
            query = query.Where(w => w.Stock!.EmployeeIds.Contains(_workContext.EmployeeId ?? Guid.Empty));

        if (message.StockId != null)
            query = query.Where(w => w.StockId == message.StockId);

        if (!string.IsNullOrEmpty(message.Query))
        {
            var searchQuery = message.Query.ToLowerInvariant().Trim();
            query = query.Where(w => w.Name.ToLower().Contains(searchQuery));
        }
        if (message.From != null && message.To != null)
        {
            var fromDate = new DateTime(message.From.Value.Ticks, DateTimeKind.Local);
            var toDate = new DateTime(message.To.Value.Ticks, DateTimeKind.Local);
            query = query.Where(w => w.OutcomeDate >= fromDate && w.OutcomeDate <= toDate);
        }

        return await query.OrderByDescending(a => a.OutcomeDate)
                    .ToPagedListAsync<Outcome, OutcomeModel>(message.Paging, _mapper);
    }
}
