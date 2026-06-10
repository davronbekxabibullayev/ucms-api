namespace Ucms.Stock.Api.Application.Consumers.Manufacturer;

using System.Threading;
using AutoMapper;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetManufacturersMessage(string? Query, PagingRequest Request) : IRequest<PagedList<ManufacturerModel>>;

public class GetManufacturersConsumer : RequestHandler<GetManufacturersMessage, PagedList<ManufacturerModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetManufacturersConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<PagedList<ManufacturerModel>> Handle(GetManufacturersMessage message, CancellationToken cancellationToken)
    {
        return await GetManufacturers(message.Query, message.Request);
    }

    private async Task<PagedList<ManufacturerModel>> GetManufacturers(string? query, PagingRequest? paging)
    {
        var manufacturersQuery = _dbContext.Manufacturers
            .OrderBy(x => x.Name)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            query = query.ToLowerInvariant().Trim();
            manufacturersQuery = manufacturersQuery.Where(x =>
                x.Name.ToLower().Contains(query)
                || x.NameRu.ToLower().Contains(query)
                || x.NameKa!.ToLower().Contains(query)
                || x.NameEn!.ToLower().Contains(query)
                || x.Code!.ToLower().Contains(query));
        }

        return await manufacturersQuery.ToPagedListAsync<Manufacturer, ManufacturerModel>(paging, _mapper);
    }
}
