namespace Ucms.Application.DTOs.Requests.Products;

using QueryForge.Abstractions;

public class GetOrganizationProductsRequest : PagedRequest
{
    public Guid? OrganizationId { get; set; }
    public Guid? StockId { get; set; }
}
