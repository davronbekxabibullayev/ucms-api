namespace Ucms.Application.Abstractions.Auth;

using System.Security.Claims;
using Ucms.Domain.Entities.Identity;

public interface ITokenService
{
    /// <summary>
    /// Foydalanuvchi uchun JWT access token yaratadi
    /// </summary>
    string GenerateAccessToken(User user, IList<string> roles);

    /// <summary>
    /// Tasodifiy refresh token yaratadi
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Muddati o'tgan tokendan ClaimsPrincipal oladi (tekshirmasdan)
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// Access tokenning amal qilish muddatini qaytaradi
    /// </summary>
    DateTimeOffset GetAccessTokenExpiry();

    /// <summary>
    /// Refresh tokenning amal qilish muddatini qaytaradi
    /// </summary>
    DateTimeOffset GetRefreshTokenExpiry();
}
