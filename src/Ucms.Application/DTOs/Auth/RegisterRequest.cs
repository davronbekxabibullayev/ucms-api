namespace Ucms.Application.DTOs.Auth;

public record RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string? FullName = null,
    Guid? OrganizationId = null
);
