namespace Ucms.Application.DTOs.Requests.Manufacturers;

using QueryForge.Abstractions;

public class GetManufacturersRequest : PagedRequest
{
    public string? Query { get; set; }
}
