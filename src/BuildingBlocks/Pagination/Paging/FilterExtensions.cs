namespace Paging;

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Paging.Core;
using Paging.Models;

public static class FilterExtensions
{
    public static async Task<PagedList<TDestination>> ToPagedListAsync<T, TDestination>(this IQueryable<T> query, FilteringRequest? request, Func<IQueryable<T>, IQueryable<TDestination>> projection)
    {
        query = query.AsFilterable(request, out var total);

        var result = await projection(query).ToListAsync();

        return new PagedList<TDestination>(result, total);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, FilteringRequest? request)
    {
        var result = await query.AsFilterable(request, out var total).ToListAsync();

        return new PagedList<T>(result, total);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, PagingRequest? request)
    {
        var result = await query.AsPageable(request, out var total).ToListAsync();

        return new PagedList<T>(result, total);
    }

    public static IQueryable<T> AsPageable<T>(this IQueryable<T> query, PagingRequest? request, out int totalRecord)
    {
        request ??= new PagingRequest();

        var tableFilterManager = new TableFilterManager<T>(query);

        query = tableFilterManager.GetResult();

        query = query.ApplySort(request.SortField, request.SortOrder);

        totalRecord = query.Count();

        query = ApplyPagination(query, request);

        return query;
    }

    public static IQueryable<T> AsFilterable<T>(this IQueryable<T> query, FilteringRequest? request, out int totalRecord)
    {
        request ??= new FilteringRequest();

        var tableFilterManager = new TableFilterManager<T>(query);

        ApplyFilter(request, tableFilterManager);

        query = tableFilterManager.GetResult();

        query = query.ApplySort(request.SortField, request.SortOrder);

        totalRecord = query.Count();

        query = ApplyPagination(query, request);

        return query;
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string? orderBy, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return source;

        var type = typeof(T);
        var properties = orderBy.Split('.');
        var parameter = Expression.Parameter(type, "p");
        Expression propertyAccess = parameter;

        foreach (var property in properties)
        {
            var prop = type.GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return source;

            propertyAccess = Expression.MakeMemberAccess(propertyAccess, prop);
            type = prop.PropertyType;
        }

        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        var methodName = sortOrder > 0 ? "OrderBy" : "OrderByDescending";
        var resultExpression = Expression.Call(typeof(Queryable), methodName, [source.ElementType, type], source.Expression, Expression.Quote(orderByExpression));

        return source.Provider.CreateQuery<T>(resultExpression);
    }

    public static IQueryable<T> AsFilterable<T>(this IQueryable<T> query, FilteringRequest? request)
    {
        return query.AsFilterable(request, out var _);
    }

    private static void ApplyFilter<T>(FilteringRequest request, TableFilterManager<T> tableFilterManager)
    {
        if (request.Filters == null || request.Filters.Count == 0)
            return;

        foreach (var filterContext in request.Filters)
        {
            var filterPayload = $"{filterContext.Value}";

            if (string.IsNullOrEmpty(filterPayload))
                continue;

            var filterToken = JToken.Parse(filterPayload);
            switch (filterToken)
            {
                case JArray:
                {
                    var filters = filterToken.ToObject<List<FilterMeta>>();
                    if (filters != null)
                        tableFilterManager.FiltersDataSet(filterContext.Key, filters);
                    break;
                }
                case JObject:
                {
                    var filter = filterToken.ToObject<FilterMeta>();
                    if (filter != null)
                        tableFilterManager.FilterDataSet(filterContext.Key, filter);
                    break;
                }

                default:
                    break;
            }
        }

        tableFilterManager.ExecuteFilter();
    }

    private static IQueryable<T> ApplyPagination<T>(IQueryable<T> query, IPagingRequest request)
    {
        return query.Skip(request.First).Take(request.Rows);
    }
}
