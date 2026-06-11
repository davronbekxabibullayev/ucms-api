namespace Ucms.Application.Handlers.StockDemand;

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

public record GetRequestedDemandsMessage(
    PagedRequest Paging,
    DateTime? From,
    DateTime? To,
    string? Name) : IRequest<PagedResult<RequestedDemandModel>>;

public class GetRequestedDemandsConsumer : RequestHandler<GetRequestedDemandsMessage, PagedResult<RequestedDemandModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IPermissionProvider _permissionProvider;

    public GetRequestedDemandsConsumer(
        IUcmsDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext,
        IPermissionProvider permissionProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedResult<RequestedDemandModel>> Handle(GetRequestedDemandsMessage message,
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
            .ToPagedResultAsync<StockDemand, RequestedDemandModel>(message.Paging, _mapper, cancellationToken);
    }
}
