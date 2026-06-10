namespace Ucms.Application.Handlers.Supplier;

using AutoMapper;
using QueryForge.Abstractions;
using QueryForge.Models;
using Ucms.Application.DTOs.Models;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using QueryForge.Extensions;
using Ucms.Application.Abstractions.Mediator;

public record GetSuppliersMessage(string? Query, PagedRequest Request) : IRequest<PagedResult<SupplierModel>>;

public class GetSuppliersConsumer : RequestHandler<GetSuppliersMessage, PagedResult<SupplierModel>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetSuppliersConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedResult<SupplierModel>> Handle(GetSuppliersMessage message,
        CancellationToken cancellationToken)
    {
        return await GetSuppliers(message.Query, message.Request);
    }

    private async Task<PagedResult<SupplierModel>> GetSuppliers(string? query, PagedRequest? paging)
    {
        var suppliersQuery = _dbContext.Suppliers
            .OrderBy(x => x.Name)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            query = query.ToLowerInvariant().Trim();
            suppliersQuery = suppliersQuery.Where(x =>
                x.Name.ToLower().Contains(query)
                || x.NameRu.ToLower().Contains(query)
                || x.NameKa!.ToLower().Contains(query)
                || x.NameEn!.ToLower().Contains(query)
                || x.Code!.ToLower().Contains(query));
        }

        return await suppliersQuery.ToPagedResultAsync<Supplier, SupplierModel>(paging, _mapper);
    }
}
