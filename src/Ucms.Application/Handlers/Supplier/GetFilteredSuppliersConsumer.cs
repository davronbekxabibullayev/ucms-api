namespace Ucms.Application.Handlers.Supplier;

using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetFilteredSuppliersMessage(PagedRequest Filter) : IRequest<PagedResult<SupplierModel>>;

public class GetFilteredSuppliersConsumer : RequestHandler<GetFilteredSuppliersMessage, PagedResult<SupplierModel>>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFilteredSuppliersConsumer(IUcmsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<SupplierModel>> Handle(GetFilteredSuppliersMessage message,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Suppliers
            .OrderBy(c => c.Name)
            .ToPagedResultAsync<Supplier, SupplierModel>(message.Filter, _mapper, cancellationToken);
    }
}
