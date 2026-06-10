namespace Ucms.Application.Handlers.Manufacturer;

using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetFilteredManufacturersMessage(PagedRequest Filter) : IRequest<PagedResult<ManufacturerModel>>;

public class GetFilteredManufacturersConsumer : RequestHandler<GetFilteredManufacturersMessage, PagedResult<ManufacturerModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFilteredManufacturersConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<ManufacturerModel>> Handle(
        GetFilteredManufacturersMessage message, CancellationToken cancellationToken)
    {
        return await _dbContext.Manufacturers
            .OrderBy(c => c.Name)
            .ToPagedResultAsync<Manufacturer, ManufacturerModel>(message.Filter, _mapper, cancellationToken);
    }
}
