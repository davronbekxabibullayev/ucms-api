namespace Ucms.Stock.Contracts.Requests.Manufacturers;

public class GetManufacturersRequest : PagingRequest
{
    public string? Query { get; set; }
}
