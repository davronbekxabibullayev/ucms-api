namespace Ucms.Application.DTOs.Requests.Skus;

using QueryForge.Abstractions;

using Ucms.Domain.Enums;

public class GetSkusRequest : PagedRequest
{
    public string? Seria { get; set; }
    public string? Query { get; set; }
    public ProductType? Type { get; set; }
}
