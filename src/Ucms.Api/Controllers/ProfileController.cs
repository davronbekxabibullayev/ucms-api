namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Profile;

/// <summary>
/// Joriy foydalanuvchi profilini boshqarish.
/// Управление профилем текущего пользователя.
/// </summary>
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

    /// <summary>
    /// Joriy foydalanuvchi profil ma'lumotlarini olish.
    /// Получить данные профиля текущего пользователя.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var data = await getProfile.HandleAsync(new(), ct);
        return data is null ? Unauthorized() : Ok(data);
    }

    /// <summary>
    /// Joriy foydalanuvchi profil ma'lumotlarini yangilash.
    /// Обновить данные профиля текущего пользователя.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req, CancellationToken ct)
    {
        var (unauthorized, errors) = await updateProfile.HandleAsync(
            new(req.FullName, req.PhoneNumber, req.Email), ct);
        if (unauthorized)       return Unauthorized();
        if (errors is not null) return BadRequest(new { errors });
        return NoContent();
    }

    /// <summary>
    /// Parolni o'zgartirish.
    /// Изменить пароль.
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var (unauthorized, errors) = await changePassword.HandleAsync(
            new(req.CurrentPassword, req.NewPassword), ct);
        if (unauthorized)       return Unauthorized();
        if (errors is not null) return BadRequest(new { errors });
        return Ok(new { message = "Parol muvaffaqiyatli o'zgartirildi. / Пароль успешно изменён." });
    }
}
