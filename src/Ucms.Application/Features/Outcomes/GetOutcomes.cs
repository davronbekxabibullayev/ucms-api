namespace Ucms.Application.Features.Outcomes;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;

public static class GetOutcomes
{
    public record Query;

    public sealed class Handler(IUcmsDbContext db, IMapper mapper, IWorkContext workContext)
    {
        public async Task<List<OutcomeModel>> HandleAsync(Query q, CancellationToken ct)
        {
            var outcomes = await db.Outcomes.Include(i => i.Stock)
                .Where(w => w.Stock!.OrganizationId == workContext.TenantId)
                .OrderByDescending(a => a.OutcomeDate).ToListAsync(ct);
            return mapper.Map<List<OutcomeModel>>(outcomes);
        }
    }
}
