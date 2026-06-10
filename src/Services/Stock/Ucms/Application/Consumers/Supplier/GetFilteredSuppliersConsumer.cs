namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Models;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredSuppliersMessage(FilteringRequest Filter) : IRequest<TableDataResult<List<SupplierModel>>>;

public class GetFilteredSuppliersConsumer : RequestHandler<GetFilteredSuppliersMessage, TableDataResult<List<SupplierModel>>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFilteredSuppliersConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<TableDataResult<List<SupplierModel>>> Handle(GetFilteredSuppliersMessage message,
        CancellationToken cancellationToken)
    {
        var queryResult = await _dbContext.Suppliers
            .OrderBy(c => c.Name)
            .AsFilterable(message.Filter, out var total)
            .ToListAsync(cancellationToken);

        var suppliers = _mapper.Map<List<SupplierModel>>(queryResult);

        return new TableDataResult<List<SupplierModel>>
        {
            Data = suppliers,
            Total = total
        };
    }
}
