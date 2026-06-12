namespace Ucms.Application.Features.Suppliers;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Persistence;

public static class FindSupplierByCode
{
    public record Query(string Code);

    public sealed class Handler(IUcmsDbContext db, IMapper mapper)
    {
        public async Task<SupplierModel?> HandleAsync(Query q, CancellationToken ct)
        {
            var entity = await db.Suppliers.FirstOrDefaultAsync(f => f.Code == q.Code, ct);
            return entity is null ? null : mapper.Map<SupplierModel>(entity);
        }
    }
}
