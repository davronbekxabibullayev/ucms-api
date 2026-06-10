namespace Paging.Core;

using System.Collections.Generic;
using System.Linq;
using Paging.Models;
using Paging.Utils;

/// <summary>
/// Class of PrimeNG table filter manager for Entity
/// </summary>
public class TableFilterManager<TEntity> : ITableFilterManager<TEntity>
{

    private readonly LinqOperator<TEntity> _linqOperator;

    public TableFilterManager(IQueryable<TEntity> dataSet)
    {
        _linqOperator = new LinqOperator<TEntity>(dataSet);
    }

    /// <summary>
    /// Set multiple condition for ordering data set to LINQ Operation context
    /// </summary>
    public void MultipleOrderDataSet(ISortingRequest request)
    {
        request.MultiSortMeta?.Select((value, i) => new { i, value }).ToList().ForEach(o =>
          {
              if (string.IsNullOrEmpty(o.value.Field))
                  return;

              switch (o.value.Order)
              {
                  case (int)SortingType.OrderByDesc:
                      if (o.i == 0)
                          _linqOperator.OrderByDescending(o.value.Field.FirstCharToUpper());
                      else
                          _linqOperator.ThenByDescending(o.value.Field.FirstCharToUpper());
                      break;

                  case (int)SortingType.OrderByAsc:
                  default:
                      if (o.i == 0)
                          _linqOperator.OrderBy(o.value.Field.FirstCharToUpper());
                      else
                          _linqOperator.ThenBy(o.value.Field.FirstCharToUpper());
                      break;
              }
          });
    }

    /// <summary> 
    /// Set single condition for ordering data set to LINQ Operation context
    /// </summary>
    public void SingleOrderDataSet(ISortingRequest request)
    {
        if (string.IsNullOrEmpty(request.SortField))
            return;

        var field = string.Join('.', request.SortField.Split('.').Select(k => k.FirstCharToUpper()));

        switch (request.SortOrder)
        {
            case (int)SortingType.OrderByDesc:
                _linqOperator.OrderByDescending(field);
                break;

            case (int)SortingType.OrderByAsc:
            default:
                _linqOperator.OrderBy(field);
                break;
        }
    }

    /// <summary>
    /// Set filter condition data to LINQ Operation context
    /// </summary>
    public void FilterDataSet(string key, FilterMeta value)
    {
        var operatorType = OperatorTypes.ConvertToOperatorType(value.Operator);

        BaseFilterDataSet(key, value, operatorType);
    }

    /// <summary>
    /// The base method for set filter condition data to LINQ Operation context
    /// </summary>
    private void BaseFilterDataSet(string key, FilterMeta value, OperatorType operatorAction)
    {
        if (value.Value == null)
            return;

        var propertyName = string.Join('.', key.Split('.').Select(k => k.FirstCharToUpper()));

        switch (value.MatchMode)
        {
            case MatchModeTypes.StartsWith:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantStartsWith, operatorAction);
                break;

            case MatchModeTypes.Contains:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantContains, operatorAction);
                break;

            case MatchModeTypes.In:
                _linqOperator.AddFilterListProperty(propertyName, value.Value, operatorAction);
                break;

            case MatchModeTypes.EndsWith:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantEndsWith, OperatorType.None);
                break;

            case MatchModeTypes.Equals:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantEquals, operatorAction);
                break;

            case MatchModeTypes.NotContains:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantContains, OperatorType.None, true);
                break;

            case MatchModeTypes.NotEquals:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantEquals, operatorAction, true);
                break;
            case MatchModeTypes.DateIs:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantDateIs, operatorAction);
                break;
            case MatchModeTypes.DateIsNot:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantDateIs, operatorAction, true);
                break;
            case MatchModeTypes.DateBefore:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantBefore, operatorAction);
                break;
            case MatchModeTypes.DateAfter:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantAfter, operatorAction);
                break;
            case MatchModeTypes.LessThan:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantLessThan, operatorAction);
                break;
            case MatchModeTypes.LessOrEqualsThan:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantLessThanOrEqual, operatorAction);
                break;
            case MatchModeTypes.GreaterThan:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantGreaterThan, operatorAction);
                break;
            case MatchModeTypes.GreaterOrEqualsThan:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantGreaterThanOrEqual, operatorAction);
                break;

            default:
                _linqOperator.AddFilterProperty(propertyName, value.Value, LinqOperatorConstants.ConstantEquals, operatorAction);
                break;
        }
    }

    /// <summary>
    /// Set multiple filter condition data to LINQ Operation context
    /// </summary>
    public void FiltersDataSet(string key, IEnumerable<FilterMeta> values)
    {
        foreach (var filterContext in values)
        {
            var operatorType = OperatorTypes.ConvertToOperatorType(filterContext.Operator);
            BaseFilterDataSet(key, filterContext, operatorType);
        }
    }

    /// <summary>
    /// Invoke filter data set from filter context setting
    /// </summary>
    public void ExecuteFilter()
    {
        _linqOperator.WhereExecute();
    }

    /// <summary>
    /// Get the filter result
    /// </summary>
    public IQueryable<TEntity> GetResult()
    {
        return _linqOperator.GetResult();
    }
}
