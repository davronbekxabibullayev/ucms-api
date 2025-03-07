namespace Paging;

using System.Collections.Generic;
using Paging.Models;

public class PagingRequest : IPagingRequest, ISortingRequest
{
    public int First { get; set; }

    public int Rows { get; set; } = 100;

    public string? SortField { get; set; }

    public int SortOrder { get; set; }

    public List<SortMeta>? MultiSortMeta { get; set; }
}
