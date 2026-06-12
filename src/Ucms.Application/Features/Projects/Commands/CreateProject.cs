namespace Ucms.Application.Features.Projects;

using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities;
using Ucms.Domain.Enums;

public static class CreateProject
{
    public record Command(
        string Name, string? Address, string? Description,
        string? ContractNumber, DateTimeOffset? ContractDate,
        DateTimeOffset? StartDate, DateTimeOffset? EndDate);

    public record Result(Guid Id, string Name);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<Result?> HandleAsync(Command cmd, CancellationToken ct)
        {
            var orgId = ctx.IsOwner ? (Guid?)null : ctx.OrganizationId;
            if (!orgId.HasValue) return null;

            var now    = DateTimeOffset.UtcNow;
            var userId = ctx.UserId ?? Guid.Empty;

            var project = new Project
            {
                Id             = Guid.NewGuid(),
                OrganizationId = orgId.Value,
                Name           = cmd.Name,
                Address        = cmd.Address,
                Description    = cmd.Description,
                ContractNumber = cmd.ContractNumber,
                ContractDate   = cmd.ContractDate,
                StartDate      = cmd.StartDate,
                EndDate        = cmd.EndDate,
                Status         = ProjectStatus.Planning,
                IsDeleted      = false,
                CreatedAt      = now, UpdatedAt = now,
                CreatedBy      = userId, UpdatedBy = userId,
            };

            await db.Projects.AddAsync(project, ct);
            await db.SaveChangesAsync(ct);
            return new Result(project.Id, project.Name);
        }
    }
}
