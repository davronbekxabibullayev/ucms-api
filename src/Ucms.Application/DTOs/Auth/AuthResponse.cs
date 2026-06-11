namespace Ucms.Application.DTOs.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    string UserName,
    string? FullName,
    IReadOnlyList<string> Roles
);
