namespace Paging;

using Paging.Models;

public interface ISortingRequest
{
    public string? SortField { get; set; }

    public int SortOrder { get; set; }

    public List<SortMeta>? MultiSortMeta { get; set; }
}
