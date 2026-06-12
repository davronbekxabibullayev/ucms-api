namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Profile;

[ApiController]
[Route("api/profile")]
[Tags("Profile")]
[Authorize]
public class ProfileController(
    GetProfile.Handler     getProfile,
    UpdateProfile.Handler  updateProfile,
    ChangePassword.Handler changePassword) : ControllerBase
{
    public record UpdateProfileRequest(string? FullName, string? PhoneNumber, string? Email);

    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var data = await getProfile.HandleAsync(new(), ct);
        return data is null ? Unauthorized() : Ok(data);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req, CancellationToken ct)
    {
        var (unauthorized, errors) = await updateProfile.HandleAsync(
            new(req.FullName, req.PhoneNumber, req.Email), ct);
        if (unauthorized)       return Unauthorized();
        if (errors is not null) return BadRequest(new { errors });
        return NoContent();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var (unauthorized, errors) = await changePassword.HandleAsync(
            new(req.CurrentPassword, req.NewPassword), ct);
        if (unauthorized)       return Unauthorized();
        if (errors is not null) return BadRequest(new { errors });
        return Ok(new { message = "Parol muvaffaqiyatli o'zgartirildi" });
    }
}
