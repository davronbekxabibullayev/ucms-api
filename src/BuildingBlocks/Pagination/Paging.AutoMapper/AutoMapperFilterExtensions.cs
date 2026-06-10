namespace Paging.AutoMapper;

using global::AutoMapper;
using Microsoft.EntityFrameworkCore;

public static class AutoMapperFilterExtensions
{
    public static async Task<PagedList<TDestination>> ToPagedListAsync<T, TDestination>(this IQueryable<T> query, PagingRequest? request, IMapper mapper)
    {
        FilteringRequest? filtering = null;

        if (request != null)
        {
            filtering = new FilteringRequest
            {
                First = request.First,
                MultiSortMeta = request.MultiSortMeta,
                Rows = request.Rows,
                SortField = request.SortField,
                SortOrder = request.SortOrder,
            };
        }

        query = query.AsFilterable(filtering, out var total);

        var result = await query.ToListAsync();

        var mappedResult = mapper.Map<List<T>, List<TDestination>>(result);

        return new PagedList<TDestination>(mappedResult, total);
    }

    public static async Task<PagedList<TDestination>> ToPagedListAsync<T, TDestination>(this IQueryable<T> query, FilteringRequest? request, IMapper mapper)
    {
        query = query.AsFilterable(request, out var total);

        var result = await query.ToListAsync();

        var mappedResult = mapper.Map<List<T>, List<TDestination>>(result);

        return new PagedList<TDestination>(mappedResult, total);
    }
}
