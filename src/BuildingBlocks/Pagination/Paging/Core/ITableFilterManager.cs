namespace Paging.Core;

using Paging.Models;

public interface ITableFilterManager<out TEntity>
{
    void MultipleOrderDataSet(ISortingRequest tableFilterPayload);
    void SingleOrderDataSet(ISortingRequest tableFilterPayload);
    void FilterDataSet(string key, FilterMeta value);
    void FiltersDataSet(string key, IEnumerable<FilterMeta> values);
    void ExecuteFilter();
    IQueryable<TEntity> GetResult();
}
