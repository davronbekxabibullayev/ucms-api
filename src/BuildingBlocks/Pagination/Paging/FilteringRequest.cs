namespace Paging;

public class FilteringRequest : PagingRequest, IFilteringRequest
{
    public Dictionary<string, object>? Filters { get; set; }
}
