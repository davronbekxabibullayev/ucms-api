namespace Ucms.Application.Abstractions;

public interface IWorkContext
{
    public Guid? TenantId { get; }
    public Guid? EmployeeId { get; }
    public string? UserId { get; }
    public string? UserName { get; }
    public string? OrganizationName { get; }
    public bool IsAdmin { get; }
}
