namespace Paging;

using System.Collections.Generic;

public abstract class PagedList
{
    public static PagedList<T> Create<T>(List<T> data, int total)
    {
        return new PagedList<T>(data, total);
    }
}

public class PagedList<T>
{
    public PagedList(List<T> data, int total)
    {
        Data = data;
        Total = total;
    }

    public List<T> Data { get; set; }

    public int Total { get; set; }
}
