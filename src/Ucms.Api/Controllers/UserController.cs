namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities.Identity;

[ApiController]
[Route("api/users")]
[Tags("User")]
[Authorize]
public class UserController(
    IUcmsDbContext db,
    UserManager<User> userManager,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record CreateUserRequest(
        string UserName,
        string Email,
        string Password,
        string? FullName,
        string? PhoneNumber,
        List<string> Roles);

    public record UpdateUserRequest(
        string? FullName,
        string? PhoneNumber,
        string? Email);

    public record SetRolesRequest(List<string> Roles);

    // ── Helpers ────────────────────────────────────────────────────────────────

    // Owner foydalanuvchilar barcha tashkilotlar foydalanuvchilarini ko'radi
    private Guid? MyOrgId() => ctx.IsOwner ? null : ctx.OrganizationId;

    private bool CanManage(Guid? userOrgId) =>
        ctx.IsOwner || (userOrgId.HasValue && ctx.OrganizationId == userOrgId);

    // ── GET /api/users ─────────────────────────────────────────────────────────

    /// <summary>
    /// Tashkilot foydalanuvchilari ro'yxati
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? organizationId,
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        var targetOrgId = ctx.IsOwner ? organizationId : ctx.OrganizationId;

        var query = db.Users
            .Where(u => !u.IsDeleted);

        if (targetOrgId.HasValue)
            query = query.Where(u => u.OrganizationId == targetOrgId.Value);
        else if (!ctx.IsOwner)
            return Forbid();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u =>
                u.UserName!.Contains(search) ||
                (u.FullName != null && u.FullName.Contains(search)) ||
                u.Email!.Contains(search));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(u => u.FullName ?? u.UserName)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.FullName,
                u.Email,
                u.PhoneNumber,
                u.OrganizationId,
                u.CreatedAt,
                Roles = db.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name)
                    .ToList(),
            })
            .ToListAsync(ct);

        return Ok(new { total, page, size, items });
    }

    // ── GET /api/users/{id} ────────────────────────────────────────────────────

    /// <summary>
    /// Foydalanuvchi tafsilotlari
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await db.Users
            .Where(u => u.Id == id && !u.IsDeleted)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.FullName,
                u.Email,
                u.PhoneNumber,
                u.OrganizationId,
                u.CreatedAt,
                u.UpdatedAt,
                Roles = db.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name)
                    .ToList(),
            })
            .FirstOrDefaultAsync(ct);

        if (user is null) return NotFound();
        if (!CanManage(user.OrganizationId)) return Forbid();

        return Ok(user);
    }

    // ── POST /api/users ────────────────────────────────────────────────────────

    /// <summary>
    /// Yangi foydalanuvchi yaratish
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        var orgId = MyOrgId();
        if (!ctx.IsOwner && !orgId.HasValue)
            return BadRequest(new { message = "Foydalanuvchiga tashkilot biriktirilmagan" });

        var now    = DateTimeOffset.UtcNow;
        var userId = ctx.UserId ?? Guid.Empty;

        var user = new User
        {
            Id             = Guid.NewGuid(),
            UserName       = req.UserName,
            Email          = req.Email,
            NormalizedEmail = req.Email.ToUpperInvariant(),
            FullName       = req.FullName,
            PhoneNumber    = req.PhoneNumber,
            OrganizationId = orgId,
            IsDeleted      = false,
            CreatedAt      = now,
            UpdatedAt      = now,
            CreatedBy      = userId,
            UpdatedBy      = userId,
        };

        var result = await userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        if (req.Roles.Count > 0)
            await userManager.AddToRolesAsync(user, req.Roles);

        return Ok(new { user.Id, user.UserName, user.Email });
    }

    // ── PUT /api/users/{id} ────────────────────────────────────────────────────

    /// <summary>
    /// Foydalanuvchi ma'lumotlarini yangilash
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null || user.IsDeleted) return NotFound();
        if (!CanManage(user.OrganizationId)) return Forbid();

        if (req.FullName is not null)   user.FullName    = req.FullName;
        if (req.PhoneNumber is not null) user.PhoneNumber = req.PhoneNumber;
        if (req.Email is not null)
        {
            user.Email           = req.Email;
            user.NormalizedEmail = req.Email.ToUpperInvariant();
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        user.UpdatedBy = ctx.UserId ?? Guid.Empty;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return NoContent();
    }

    // ── PATCH /api/users/{id}/roles ────────────────────────────────────────────

    /// <summary>
    /// Foydalanuvchi rollarini o'rnatish (to'liq almashtirish)
    /// </summary>
    [HttpPatch("{id:guid}/roles")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> SetRoles(Guid id, [FromBody] SetRolesRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null || user.IsDeleted) return NotFound();
        if (!CanManage(user.OrganizationId)) return Forbid();

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRolesAsync(user, req.Roles);

        return Ok(new { roles = req.Roles });
    }

    // ── PATCH /api/users/{id}/toggle-active ────────────────────────────────────

    /// <summary>
    /// Foydalanuvchini faollashtirish / bloklash (LockoutEnd orqali)
    /// </summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null || user.IsDeleted) return NotFound();
        if (!CanManage(user.OrganizationId)) return Forbid();

        var isLocked = await userManager.IsLockedOutAsync(user);
        if (isLocked)
        {
            await userManager.SetLockoutEndDateAsync(user, null);
            return Ok(new { active = true, message = "Foydalanuvchi faollashtirildi" });
        }
        else
        {
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return Ok(new { active = false, message = "Foydalanuvchi bloklandi" });
        }
    }

    // ── DELETE /api/users/{id} ─────────────────────────────────────────────────

    /// <summary>
    /// Foydalanuvchini o'chirish (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null || user.IsDeleted) return NotFound();
        if (!CanManage(user.OrganizationId)) return Forbid();

        // O'zini o'chira olmaydi
        if (user.Id == ctx.UserId)
            return BadRequest(new { message = "O'zingizni o'chira olmaysiz" });

        user.IsDeleted = true;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        user.UpdatedBy = ctx.UserId ?? Guid.Empty;

        await userManager.UpdateAsync(user);

        return NoContent();
    }

    // ── GET /api/users/roles ───────────────────────────────────────────────────

    /// <summary>
    /// Mavjud rollar ro'yxati
    /// </summary>
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var roles = await db.Roles
            .OrderBy(r => r.Name)
            .Select(r => new { r.Id, r.Name, r.Description })
            .ToListAsync(ct);

        return Ok(roles);
    }
}
