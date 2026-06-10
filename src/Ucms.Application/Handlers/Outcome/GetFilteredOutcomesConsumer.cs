namespace Ucms.Application.Handlers.Outcome;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Authorization;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;
using Ucms.Application.Abstractions.Constants;

public record GetFilteredOutcomesMessage(
    PagedRequest Paging,
    Guid? StockId,
    string? Query,
    DateTime? From,
    DateTime? To) : IRequest<PagedResult<OutcomeModel>>;

public class GetFilteredOutcomesConsumer : RequestHandler<GetFilteredOutcomesMessage, PagedResult<OutcomeModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IMapper _mapper;
    private readonly IPermissionProvider _permissionProvider;

    public GetFilteredOutcomesConsumer(
        IAppDbContext dbContext,
        IWorkContext workContext,
        IMapper mapper,
        IPermissionProvider permissionProvider)
    {
        _dbContext = dbContext;
        _workContext = workContext;
        _mapper = mapper;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedResult<OutcomeModel>> Handle(GetFilteredOutcomesMessage message,
        CancellationToken cancellationToken)
    {
        return await GetOutcomes(message, cancellationToken);
    }

    private async Task<PagedResult<OutcomeModel>> GetOutcomes(GetFilteredOutcomesMessage message, CancellationToken cancellationToken)
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
                    .ToPagedResultAsync<Outcome, OutcomeModel>(message.Paging, _mapper, cancellationToken);
    }
}
