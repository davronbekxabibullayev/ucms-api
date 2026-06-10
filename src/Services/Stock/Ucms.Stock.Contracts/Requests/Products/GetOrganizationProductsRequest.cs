namespace Ucms.Stock.Contracts.Requests.Products;

public class GetOrganizationProductsRequest : PagingRequest
{
    public Guid? OrganizationId { get; set; }
    public Guid? StockId { get; set; }
}
