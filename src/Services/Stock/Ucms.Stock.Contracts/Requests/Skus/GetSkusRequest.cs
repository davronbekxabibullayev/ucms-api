namespace Ucms.Stock.Contracts.Requests.Skus;

using Ucms.Stock.Domain.Models.Enums;

public class GetSkusRequest : PagingRequest
{
    public string? Seria { get; set; }
    public string? Query { get; set; }
    public ProductType? Type { get; set; }
}
