namespace Ucms.Stock.Api.Application.Consumers.Stock;

using AutoMapper;
using Devhub.Authorization;
using Ucms.Common.Paging;
using Ucms.Core.Constants;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredStocksMessage(PagingRequest Paging) : IRequest<PagedList<StockModel>>;

public class GetFilteredStocksConsumer : RequestHandler<GetFilteredStocksMessage, PagedList<StockModel>>
{
    private readonly IMapper _mapper;
    private readonly IStockDbContext _dbContext;
    private readonly IWorkContext _workContext;
    private readonly IPermissionProvider _permissionProvider;

    public GetFilteredStocksConsumer(
        IMapper mapper,
        IStockDbContext dbContext,
        IWorkContext workContext,
        IPermissionProvider permissionProvider)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _workContext = workContext;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedList<StockModel>> Handle(GetFilteredStocksMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Stocks.AsQueryable();

        if (!_workContext.IsAdmin)
            query = query.Where(w => w.OrganizationId == _workContext.TenantId);

        if (!await _permissionProvider.HasPermissionAsync(Permissions.Warehouse.AccessSettingMinimumBalanceWarehouse, cancellationToken))
            query = query.Where(w => w.EmployeeIds.Contains(_workContext.EmployeeId ?? Guid.Empty));

        return await query
            .OrderBy(c => c.Name)
            .ToPagedListAsync<Stock, StockModel>(message.Paging, _mapper);
    }
}
