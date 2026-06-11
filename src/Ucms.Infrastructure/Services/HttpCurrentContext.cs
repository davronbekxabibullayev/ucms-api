namespace Ucms.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Ucms.Application.Abstractions;

public class HttpCurrentContext(IHttpContextAccessor httpContextAccessor) : ICurrentContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User?.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? UserName => User?.FindFirstValue(ClaimTypes.Name)
                            ?? User?.FindFirstValue("name");

    public Guid? OrganizationId
    {
        get
        {
            var value = User?.FindFirstValue("organization_id")
                     ?? User?.FindFirstValue("org_id");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? OrganizationName => User?.FindFirstValue("organization_name");

    public bool IsAdmin => User?.IsInRole("Admin") ?? false;

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ?? (IReadOnlyList<string>)[];
}
