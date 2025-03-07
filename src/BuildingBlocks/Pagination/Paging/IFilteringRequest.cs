namespace Paging;

public interface IFilteringRequest : IPagingRequest
{
    Dictionary<string, object>? Filters { get; set; }
}
