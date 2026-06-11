namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ucms.Application.Abstractions;
using Ucms.Application.Persistence;
using Ucms.Domain.Entities.Identity;

[ApiController]
[Route("api/profile")]
[Tags("Profile")]
[Authorize]
public class ProfileController(
    IUcmsDbContext db,
    UserManager<User> userManager,
    ICurrentContext ctx) : ControllerBase
{
    // ── Requests ───────────────────────────────────────────────────────────────

    public record UpdateProfileRequest(
        string? FullName,
        string? PhoneNumber,
        string? Email);

    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword);

    // ── GET /api/profile ───────────────────────────────────────────────────────

    /// <summary>
    /// Joriy foydalanuvchi ma'lumotlari
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        if (ctx.UserId is null) return Unauthorized();

        var user = await db.Users
            .Where(u => u.Id == ctx.UserId && !u.IsDeleted)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.FullName,
                u.Email,
                u.PhoneNumber,
                u.OrganizationId,
                u.CreatedAt,
                Organization = u.OrganizationId == null ? null :
                    db.Organizations
                        .Where(o => o.Id == u.OrganizationId)
                        .Select(o => new { o.Id, o.Name })
                        .FirstOrDefault(),
                Roles = db.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name)
                    .ToList(),
                IsAdmin = ctx.IsAdmin,
            })
            .FirstOrDefaultAsync(ct);

        return user is null ? Unauthorized() : Ok(user);
    }

    // ── PUT /api/profile ───────────────────────────────────────────────────────

    /// <summary>
    /// O'z profilini yangilash
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req, CancellationToken ct)
    {
        if (ctx.UserId is null) return Unauthorized();

        var user = await userManager.FindByIdAsync(ctx.UserId.ToString()!);
        if (user is null || user.IsDeleted) return Unauthorized();

        if (req.FullName is not null)    user.FullName    = req.FullName;
        if (req.PhoneNumber is not null) user.PhoneNumber = req.PhoneNumber;
        if (req.Email is not null)
        {
            user.Email           = req.Email;
            user.NormalizedEmail = req.Email.ToUpperInvariant();
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        user.UpdatedBy = ctx.UserId.Value;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return NoContent();
    }

    // ── POST /api/profile/change-password ─────────────────────────────────────

    /// <summary>
    /// Parolni o'zgartirish
    /// </summary>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        if (ctx.UserId is null) return Unauthorized();

        var user = await userManager.FindByIdAsync(ctx.UserId.ToString()!);
        if (user is null || user.IsDeleted) return Unauthorized();

        var result = await userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = "Parol muvaffaqiyatli o'zgartirildi" });
    }
}
