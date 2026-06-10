namespace Ucms.Application.DTOs.Requests.Suppliers;

using QueryForge.Abstractions;

public class GetSuppliersRequest : PagedRequest
{
    public string? Query { get; set; }
}
