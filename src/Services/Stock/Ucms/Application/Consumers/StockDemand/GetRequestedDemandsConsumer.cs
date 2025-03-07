namespace Ucms.Stock.Api.Application.Consumers.StockDemand;

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

public record GetRequestedDemandsMessage(
    PagingRequest Paging,
    DateTime? From,
    DateTime? To,
    string? Name) : IRequest<PagedList<RequestedDemandModel>>;

public class GetRequestedDemandsConsumer : RequestHandler<GetRequestedDemandsMessage, PagedList<RequestedDemandModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IPermissionProvider _permissionProvider;

    public GetRequestedDemandsConsumer(
        IStockDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext,
        IPermissionProvider permissionProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedList<RequestedDemandModel>> Handle(GetRequestedDemandsMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StockDemands
            .Include(i => i.Sender)
            .Where(w => w.Sender!.OrganizationId == _workContext.TenantId);

        if (!await _permissionProvider.HasPermissionAsync(Permissions.Warehouse.AccessSettingMinimumBalanceWarehouse, cancellationToken))
            query = query.Where(w => w.Sender!.EmployeeIds.Contains(_workContext.EmployeeId ?? Guid.Empty));

        if (message.From != null && message.To != null)
            query = query.Where(w => w.DemandDate.Date >= message.From.Value.Date && w.DemandDate.Date <= message.To.Value.Date);

        if (!string.IsNullOrEmpty(message.Name))
        {
            var name = message.Name.ToLower();
            query = query.Where(w => w.Name.ToLower().Contains(name));
        }

        return await query
            .OrderBy(c => c.Name)
            .ToPagedListAsync<StockDemand, RequestedDemandModel>(message.Paging, _mapper);
    }
}
