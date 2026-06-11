namespace Ucms.Application.Handlers.Stock;

using System.Globalization;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Enums;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Authorization;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;
using Ucms.Application.Abstractions.Constants;
using Ucms.Application.Abstractions.Organization;

public record GetStocksMessage(Guid OrganizationId,
    bool? Unattached,
    StockType? StockType,
    StockCategory? StockCategory,
    string? Query,
    bool? IncludeChild) : IRequest<List<StockModel>>;

public class GetStocksConsumer : RequestHandler<GetStocksMessage, List<StockModel>>
{
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;
    private readonly IUcmsDbContext _dbContext;
    private readonly IPermissionProvider _permissionProvider;
    private readonly IOrganizationClient _organizationClient;

    public GetStocksConsumer(
        IMapper mapper,
        IWorkContext workContext,
        IUcmsDbContext dbContext,
        IPermissionProvider permissionProvider,
        IOrganizationClient organizationClient)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _workContext = workContext;
        _permissionProvider = permissionProvider;
        _organizationClient = organizationClient;
    }
    protected override async Task<List<StockModel>> Handle(GetStocksMessage message, CancellationToken cancellationToken)
    {
        var organizationId = message.OrganizationId;
        var stocks = _dbContext.Stocks.AsQueryable();

        if (_workContext.TenantId == message.OrganizationId)
        {
            if (!await _permissionProvider.HasPermissionAsync(Permissions.Warehouse.AccessAddWarehouse, cancellationToken))
            {
                stocks = stocks.Where(w => w.EmployeeIds.Contains(_workContext.EmployeeId ?? Guid.Empty));
            }
        }

        if (!string.IsNullOrEmpty(message.Query))
        {
            var query = message.Query.ToLower(CultureInfo.InvariantCulture);
            stocks = stocks.Where(a =>
                a.Name.ToLower().Contains(query) ||
                a.NameRu.ToLower().Contains(query) ||
                a.NameEn!.ToLower().Contains(query) ||
                a.NameKa!.ToLower().Contains(query) ||
                a.Code.Contains(query));
        }

        if (message.IncludeChild == true)
        {
            var organizationIds = await _organizationClient.GetOrganizationIds(includeChilds: true);
            stocks = stocks.Where(w => organizationIds.Contains(w.OrganizationId));
        }
        else
        {
            stocks = stocks.Where(a => a.OrganizationId == organizationId);
        }

        if (message.StockType != null)
        {
            stocks = stocks.Where(a => a.StockType == message.StockType);
        }

        if (message.StockCategory != null)
        {
            stocks = stocks.Where(a => a.StockCategory == message.StockCategory);
        }

        if (message.Unattached == true)
        {
            var stockIds = await _organizationClient.GetStockIds(organizationId);
            stocks = stocks.Where(a => !stockIds.Contains(a.Id));
        }

        stocks = stocks.OrderBy(a => a.Name);

        return _mapper.Map<List<StockModel>>(await stocks.ToListAsync(cancellationToken));
    }
}
