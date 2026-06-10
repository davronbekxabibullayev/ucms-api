namespace Ucms.Stock.Contracts.Requests.Suppliers;

public class GetSuppliersRequest : PagingRequest
{
    public string? Query { get; set; }
}
