namespace Paging.Models;

using System.Linq.Expressions;

public class LinqContext<TEntity>
{
    public IQueryable<TEntity> DataSet { get; set; } = default!;
    public ParameterExpression ParameterExpression { get; set; } = default!;
    public Type DataSetType { get; set; } = default!;
    public Expression<Func<TEntity, bool>> Expressions { get; set; } = default!;
}
