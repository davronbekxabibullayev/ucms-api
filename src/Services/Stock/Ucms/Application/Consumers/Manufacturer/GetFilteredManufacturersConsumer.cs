namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Common.Models;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetFilteredManufacturersMessage
    (FilteringRequest Filter) : IRequest<TableDataResult<List<ManufacturerModel>>>;

public class
    GetFilteredManufacturersConsumer : RequestHandler<GetFilteredManufacturersMessage,
        TableDataResult<List<ManufacturerModel>>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFilteredManufacturersConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<TableDataResult<List<ManufacturerModel>>> Handle(
        GetFilteredManufacturersMessage message, CancellationToken cancellationToken)
    {
        var queryResult = await _dbContext.Manufacturers
            .AsQueryable()
            .OrderBy(c => c.Name)
            .AsFilterable(message.Filter, out var total)
            .ToListAsync(cancellationToken);

        var manufacturers = _mapper.Map<List<ManufacturerModel>>(queryResult);

        var result = new TableDataResult<List<ManufacturerModel>>
        {
            Data = manufacturers,
            Total = total
        };

        return result;
    }
}
