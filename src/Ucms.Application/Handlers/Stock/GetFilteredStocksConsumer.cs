namespace Ucms.Application.Handlers.Stock;

using AutoMapper;
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

public record GetFilteredStocksMessage(PagedRequest Paging) : IRequest<PagedResult<StockModel>>;

public class GetFilteredStocksConsumer : RequestHandler<GetFilteredStocksMessage, PagedResult<StockModel>>
{
    private readonly IMapper _mapper;
    private readonly IUcmsDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IPermissionProvider _permissionProvider;

    public GetFilteredStocksConsumer(
        IMapper mapper,
        IUcmsDbContext dbContext,
        IWorkContext workContext,
        IPermissionProvider permissionProvider)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _workContext = workContext;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedResult<StockModel>> Handle(GetFilteredStocksMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Stocks.AsQueryable();

        if (!_workContext.IsAdmin)
            query = query.Where(w => w.OrganizationId == _workContext.TenantId);

        if (!await _permissionProvider.HasPermissionAsync(Permissions.Warehouse.AccessSettingMinimumBalanceWarehouse, cancellationToken))
            query = query.Where(w => w.EmployeeIds.Contains(_workContext.EmployeeId ?? Guid.Empty));

        return await query
            .OrderBy(c => c.Name)
            .ToPagedResultAsync<Stock, StockModel>(message.Paging, _mapper, cancellationToken);
    }
}
