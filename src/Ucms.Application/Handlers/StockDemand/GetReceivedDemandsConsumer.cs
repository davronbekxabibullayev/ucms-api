namespace Ucms.Application.Handlers.StockDemand;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Authorization;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;
using Ucms.Application.Abstractions.Constants;

public record GetReceivedDemandsMessage(
    PagedRequest Paging,
    DateTime? From,
    DateTime? To,
    string? Name) : IRequest<PagedResult<ReceivedDemandModel>>;

public class
    GetReceivedDemandsConsumer : RequestHandler<GetReceivedDemandsMessage, PagedResult<ReceivedDemandModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IPermissionProvider _permissionProvider;

    public GetReceivedDemandsConsumer(
        IUcmsDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext,
        IPermissionProvider permissionProvider
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
        _permissionProvider = permissionProvider;
    }

    protected override async Task<PagedResult<ReceivedDemandModel>> Handle(GetReceivedDemandsMessage message,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StockDemands
            .Include(i => i.Sender)
            .Include(i => i.StockDemandItems)
            .ThenInclude(th => th.Product)
            .Include(i => i.StockDemandItems)
            .ThenInclude(th => th.MeasurementUnit)
            .Where(w => w.DemandStatus != StockDemandStatus.Draft);

        if (!_workContext.IsAdmin)
            query = query.Where(w => w.Recipient!.OrganizationId == _workContext.TenantId);

        if (!await _permissionProvider.HasPermissionAsync(Permissions.Warehouse.AccessSettingMinimumBalanceWarehouse, cancellationToken))
            query = query.Where(w => w.Recipient!.EmployeeIds.Contains(_workContext.EmployeeId ?? Guid.Empty));

        if (message.From != null && message.To != null)
            query = query.Where(w => w.DemandDate.Date >= message.From.Value.Date && w.DemandDate.Date <= message.To.Value.Date);

        if (!string.IsNullOrEmpty(message.Name))
        {
            var name = message.Name.ToLower();
            query = query.Where(w => w.Name.ToLower().Contains(name));
        }

        return await query
            .OrderBy(c => c.Name)
            .ToPagedResultAsync<StockDemand, ReceivedDemandModel>(message.Paging, _mapper, cancellationToken);
    }
}
