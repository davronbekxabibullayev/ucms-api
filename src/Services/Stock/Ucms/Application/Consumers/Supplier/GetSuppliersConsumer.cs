namespace Ucms.Stock.Api.Application.Consumers.Supplier;

using AutoMapper;
using Ucms.Common.Paging;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Domain.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record GetSuppliersMessage(string? Query, PagingRequest Request) : IRequest<PagedList<SupplierModel>>;

public class GetSuppliersConsumer : RequestHandler<GetSuppliersMessage, PagedList<SupplierModel>>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetSuppliersConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<PagedList<SupplierModel>> Handle(GetSuppliersMessage message,
        CancellationToken cancellationToken)
    {
        return await GetSuppliers(message.Query, message.Request);
    }

    private async Task<PagedList<SupplierModel>> GetSuppliers(string? query, PagingRequest? paging)
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

        return await suppliersQuery.ToPagedListAsync<Supplier, SupplierModel>(paging, _mapper);
    }
}
