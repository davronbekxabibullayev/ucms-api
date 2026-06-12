namespace Ucms.Application.Features.Estimates;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;

public static class CreateItem
{
    public record Command(
        Guid ProjectId, Guid SectionId,
        string Name, string Unit, decimal Volume,
        decimal ClientUnitPrice, decimal BrigadeUnitPrice, int Order);

    public record Result(Guid Id, string Name);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(Result? Data, bool Forbidden, string? Error)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var orgId = await db.Projects
                .Where(p => p.Id == cmd.ProjectId && !p.IsDeleted)
                .Select(p => (Guid?)p.OrganizationId)
                .FirstOrDefaultAsync(ct);

            if (orgId is null || (!ctx.IsOwner && ctx.OrganizationId != orgId))
                return (null, orgId is not null, null);

            var sectionExists = await db.EstimateSections
                .AnyAsync(s => s.Id == cmd.SectionId && s.ProjectId == cmd.ProjectId, ct);

            if (!sectionExists)
                return (null, false, "Bo'lim ushbu loyihaga tegishli emas");

            var item = new EstimateItem
            {
                Id               = Guid.NewGuid(),
                SectionId        = cmd.SectionId,
                Name             = cmd.Name,
                Unit             = cmd.Unit,
                Volume           = cmd.Volume,
                ClientUnitPrice  = cmd.ClientUnitPrice,
                BrigadeUnitPrice = cmd.BrigadeUnitPrice,
                Order            = cmd.Order,
            };

            await db.EstimateItems.AddAsync(item, ct);
            await db.SaveChangesAsync(ct);
            return (new Result(item.Id, item.Name), false, null);
        }
    }
}
