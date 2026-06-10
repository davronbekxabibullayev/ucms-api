namespace Ucms.Application.Handlers.Manufacturer;

using System.Threading;
using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetManufacturersMessage(string? Query, PagedRequest Request) : IRequest<PagedResult<ManufacturerModel>>;

public class GetManufacturersConsumer : RequestHandler<GetManufacturersMessage, PagedResult<ManufacturerModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetManufacturersConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<PagedResult<ManufacturerModel>> Handle(GetManufacturersMessage message, CancellationToken cancellationToken)
    {
        return await GetManufacturers(message.Query, message.Request);
    }

    private async Task<PagedResult<ManufacturerModel>> GetManufacturers(string? query, PagedRequest? paging)
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

        return await manufacturersQuery.ToPagedResultAsync<Manufacturer, ManufacturerModel>(paging, _mapper);
    }
}
