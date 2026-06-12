namespace Ucms.Application.Features.WorkLogs;

using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Enums;

public static class UpdateWorkLog
{
    public record Command(
        Guid ProjectId, Guid Id,
        DateTimeOffset Date, decimal Volume, decimal BrigadeUnitPrice, string? Note);

    public sealed class Handler(IUcmsDbContext db, ICurrentContext ctx)
    {
        public async Task<(bool NotFound, bool Forbidden, string? Error)> HandleAsync(Command cmd, CancellationToken ct)
        {
            var workLog = await db.WorkLogs
                .FirstOrDefaultAsync(w => w.Id == cmd.Id && w.ProjectId == cmd.ProjectId, ct);

            if (workLog is null) return (true, false, null);

            var orgId = await db.Projects
                .Where(p => p.Id == cmd.ProjectId && !p.IsDeleted)
                .Select(p => (Guid?)p.OrganizationId)
                .FirstOrDefaultAsync(ct);

            if (!ctx.IsOwner && ctx.OrganizationId != orgId) return (false, true, null);

            if (workLog.Status != WorkLogStatus.Draft)
                return (false, false, "Faqat Draft holatidagi yozuvni o'zgartirish mumkin");

            workLog.Date             = cmd.Date;
            workLog.Volume           = cmd.Volume;
            workLog.BrigadeUnitPrice = cmd.BrigadeUnitPrice;
            workLog.TotalAmount      = cmd.Volume * cmd.BrigadeUnitPrice;
            workLog.Note             = cmd.Note;
            workLog.UpdatedAt        = DateTimeOffset.UtcNow;
            workLog.UpdatedBy        = ctx.UserId ?? Guid.Empty;

            db.WorkLogs.Update(workLog);
            await db.SaveChangesAsync(ct);
            return (false, false, null);
        }
    }
}
