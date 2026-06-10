namespace Paging.Core;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Paging.Models;
using Paging.Utils;

public class LinqOperator<TEntity> : ILinqOperator<TEntity>
{
    private readonly LinqContext<TEntity> _context;

    public LinqOperator(IQueryable<TEntity> dataSet)
    {
        _context = new LinqContext<TEntity>
        {
            DataSet = dataSet,
            DataSetType = typeof(TEntity),
            ParameterExpression = Expression.Parameter(typeof(TEntity), "e")
        };
    }

    public void AddFilterListProperty(
        string propertyName,
        object propertyValue,
        OperatorType operatorAction)
    {
        var property = _context.DataSetType.GetProperty(propertyName);
        if (property == null)
            return;

        var propertyType = property.PropertyType;
        if (propertyType == null)
            return;

        var castValue = ObjectCasterUtil.CastPropertiesTypeList(property, propertyValue);
        var methodInfo = castValue.GetType()
            .GetMethod(LinqOperatorConstants.ConstantContains, [propertyType]);
        if (methodInfo == null)
            return;

        var list = Expression.Constant(castValue);
        var value = Expression.Property(_context.ParameterExpression, propertyName);
        AddNormalExpression(operatorAction, list, methodInfo, value);
    }

    public void AddFilterProperty(
        string propertyName,
        object propertyValue,
        string extensionMethod,
        OperatorType operatorAction,
        bool isNegation = false)
    {

        var property = TypeUtils.GetProperty(_context.DataSetType, propertyName)
            ?? throw new ArgumentException($"Property ${propertyName} not does not exist");

        var propertyType = property.PropertyType;

        if (propertyType == null)
            return;

        if (!IsPropertyTypeAndFilterMatchModeValid(propertyType, extensionMethod))
            throw new ArgumentException($"Property ${propertyName} not support method ${extensionMethod}");

        var castValue = ObjectCasterUtil.CastPropertiesType(property, propertyValue);

        if (IsNullableType(propertyType))
            switch (castValue)
            {
                case DateTime:
                    ComposeLambdaForDateTimeProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                    break;
                case bool:
                    // boolean only have "equals" and "notEquals" match modes
                    ComposeEqualsLinqExpression(propertyName, operatorAction, isNegation, castValue, _context.ParameterExpression);
                    break;
                // nullable numeric type
                default:
                    ComposeLambdaForNumericProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                    break;
            }
        else
            switch (castValue)
            {
                case DateTime:
                    ComposeLambdaForDateTimeProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                    break;
                case bool:
                    ComposeEqualsLinqExpression(propertyName, operatorAction, isNegation, castValue, _context.ParameterExpression);
                    return;
                case string:
                {
                    Expression propertyAccess = TypeUtils.GetPropertyAccess(_context.DataSetType, propertyName, _context.ParameterExpression);

                    var methodInfo = propertyType.GetMethod(extensionMethod, [propertyType])
                         ?? throw new InvalidOperationException();

                    var propertyConstant = Expression.Constant(castValue);

                    // TODO: Пересмотреть. Поиск без учет реестра.
                    if (extensionMethod == nameof(string.Contains))
                    {
                        var toLowerMethodInfo = typeof(string).GetMethod("ToLower", [])!;
                        propertyAccess = Expression.Call(propertyAccess, toLowerMethodInfo);
                        propertyConstant = Expression.Constant(castValue.ToString()?.ToLower());
                    }

                    if (isNegation)
                        AddNegationExpression(operatorAction, propertyAccess, methodInfo, propertyConstant);
                    else
                        AddNormalExpression(operatorAction, propertyAccess, methodInfo, propertyConstant);
                    break;
                }
                // nullable numeric type
                default:
                    ComposeLambdaForNumericProperty(propertyName, extensionMethod, operatorAction, isNegation, castValue);
                    break;
            }

    }
    private void AddNormalExpression(
        OperatorType operatorAction,
        Expression propertyAccess,
        MethodInfo methodInfo,
        Expression converted)
    {
        var callMethod = Expression.Call(propertyAccess,
            methodInfo ?? throw new InvalidOperationException(), converted);
        AddLambdaExpression(operatorAction, callMethod);
    }

    private void AddNormalExpression(OperatorType operatorAction, Expression propertyAccess)
    {
        AddLambdaExpression(operatorAction, propertyAccess);
    }

    private void AddNegationExpression(
        OperatorType operatorAction,
        Expression propertyAccess,
        MethodInfo methodInfo,
        Expression converted)
    {
        ArgumentNullException.ThrowIfNull(propertyAccess);
        var callMethod = Expression.Not(Expression.Call(propertyAccess, methodInfo, converted));
        AddLambdaExpression(operatorAction, callMethod);
    }
    private void AddNegationExpression(OperatorType operatorAction, Expression propertyAccess)
    {
        AddLambdaExpression(operatorAction, Expression.Not(propertyAccess));

    }

    private void AddLambdaExpression(OperatorType operatorAction, Expression callMethod)
    {
        var lambda = Expression.Lambda<Func<TEntity, bool>>(callMethod, _context.ParameterExpression);
        if (_context.Expressions == null)
            _context.Expressions = lambda;
        else
            _context.Expressions = operatorAction switch
            {
                OperatorType.And => Expression.Lambda<Func<TEntity, bool>>(
                    Expression.AndAlso(_context.Expressions.Body, lambda.Body),
                    _context.ParameterExpression),
                OperatorType.Or => Expression.Lambda<Func<TEntity, bool>>(
                    Expression.OrElse(_context.Expressions.Body, lambda.Body),
                    _context.ParameterExpression),
                OperatorType.None => Expression.Lambda<Func<TEntity, bool>>(
                    Expression.AndAlso(_context.Expressions.Body, lambda.Body),
                    _context.ParameterExpression),
                _ => Expression.Lambda<Func<TEntity, bool>>(
                    Expression.AndAlso(_context.Expressions.Body, lambda.Body),
                    _context.ParameterExpression),
            };
    }

    public void WhereExecute()
    {
        _context.DataSet = _context.Expressions != null
            ? _context.DataSet.Where(_context.Expressions)
            : _context.DataSet;
    }

    public void OrderBy(string orderProperty)
    {
        BaseOrderExecute(LinqOperatorConstants.ConstantOrderBy, orderProperty);
    }

    public void OrderByDescending(string orderProperty)
    {
        BaseOrderExecute(LinqOperatorConstants.ConstantOrderByDescending, orderProperty);
    }

    public void ThenBy(string orderProperty)
    {
        BaseOrderExecute(LinqOperatorConstants.ConstantThenBy, orderProperty);
    }

    public void ThenByDescending(string orderProperty)
    {
        BaseOrderExecute(LinqOperatorConstants.ConstantThenByDescending, orderProperty);
    }

    private void BaseOrderExecute(string command, string orderByProperty)
    {
        var property = TypeUtils.GetProperty(_context.DataSetType, orderByProperty)
            ?? throw new ArgumentException($"Property ${orderByProperty} not does not exist");

        var propertyAccess = Expression.MakeMemberAccess(_context.ParameterExpression, property);

        var orderByExpression = Expression.Lambda(propertyAccess, _context.ParameterExpression);

        var resultExpression = Expression.Call(
            typeof(Queryable),
            command,
            [_context.DataSetType, property.PropertyType],
            _context.DataSet.Expression,
            Expression.Quote(orderByExpression));

        _context.DataSet = _context.DataSet.Provider.CreateQuery<TEntity>(resultExpression);
    }

    /// <summary>
    /// Checks if provided type is nullable
    /// </summary>
    /// <param name="propertyType">Type to check</param>
    /// <returns><code>True</code> if nullable, otherwise <code>False</code></returns>
    private static bool IsNullableType(Type propertyType)
    {
        return propertyType.IsGenericType
                   && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
    /// <summary>
    /// Checks if provided type is numeric type
    /// </summary>
    /// <param name="propertyType">Type to check</param>
    /// <returns><code>True</code> if nullable, otherwise <code>False</code></returns>
    private static bool IsNumericType(Type propertyType)
    {
        return propertyType == typeof(short) || propertyType == typeof(short?) || propertyType == typeof(int) || propertyType == typeof(int?) || propertyType == typeof(long) || propertyType == typeof(long?)
              || propertyType == typeof(float) || propertyType == typeof(float?) || propertyType == typeof(double) || propertyType == typeof(double?) || propertyType == typeof(decimal) || propertyType == typeof(decimal?);
    }
    /// <summary>
    /// Checks if for provided <paramref name="propertyType"/>, <paramref name="extensionMethod"/> is valid
    /// </summary>
    /// <param name="propertyType">Type of filter value instance</param>
    /// <param name="extensionMethod">Method to check for provided <paramref name="propertyType"/> </param>
    /// <returns></returns>
    private static bool IsPropertyTypeAndFilterMatchModeValid(Type propertyType, string extensionMethod)
    {
        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            var validDateTimeLinqMethods = new[] { LinqOperatorConstants.ConstantDateIs, LinqOperatorConstants.ConstantBefore, LinqOperatorConstants.ConstantAfter };
            return validDateTimeLinqMethods.Contains(extensionMethod);
        }

        if (propertyType == typeof(string))
        {
            var validStringLinqMethods = new[] { LinqOperatorConstants.ConstantEquals, LinqOperatorConstants.ConstantEndsWith, LinqOperatorConstants.ConstantContains, LinqOperatorConstants.ConstantStartsWith };
            return validStringLinqMethods.Contains(extensionMethod);
        }

        if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            return LinqOperatorConstants.ConstantEquals == extensionMethod;

        if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
            return LinqOperatorConstants.ConstantEquals == extensionMethod;

        //if (!IsNumericType(propertyType))
        //    return false;
        var validNumericMethods = new[] { LinqOperatorConstants.ConstantEquals, LinqOperatorConstants.ConstantLessThan, LinqOperatorConstants.ConstantLessThanOrEqual, LinqOperatorConstants.ConstantGreaterThan, LinqOperatorConstants.ConstantGreaterThanOrEqual };
        return validNumericMethods.Contains(extensionMethod);
    }



    /// <summary>
    /// Composes LINQ expression for numeric type of based on <paramref name="extensionMethod"/>
    /// </summary>
    /// <param name="propertyName">Name of property to compose expression</param>
    /// <param name="extensionMethod">Extension method for which expression is composed</param>
    /// <param name="operatorAction">Operator action for expression</param>
    /// <param name="isNegation">Flag that indicates if expression should be negation</param>
    /// <param name="castValue">Casted value of <paramref name="propertyName"/></param>
    private void ComposeLambdaForNumericProperty(string propertyName, string extensionMethod, OperatorType operatorAction, bool isNegation, object castValue)
    {
        LambdaExpression dynamicExpression;
        var x = _context.ParameterExpression;
        switch (extensionMethod)
        {
            case LinqOperatorConstants.ConstantEquals:
            {
                ComposeEqualsLinqExpression(propertyName, operatorAction, isNegation, castValue, x);
                break;
            }
            case LinqOperatorConstants.ConstantLessThan:
            {
                dynamicExpression = DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}<@0", castValue);
                if (isNegation)
                    AddNegationExpression(operatorAction, dynamicExpression.Body);
                else
                    AddNormalExpression(operatorAction, dynamicExpression.Body);
                break;
            }
            case LinqOperatorConstants.ConstantLessThanOrEqual:
            {
                dynamicExpression = DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}<=@0", castValue);
                if (isNegation)
                    AddNegationExpression(operatorAction, dynamicExpression.Body);
                else
                    AddNormalExpression(operatorAction, dynamicExpression.Body);
                break;
            }
            case LinqOperatorConstants.ConstantGreaterThan:
            {
                dynamicExpression = DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}>@0", castValue);
                if (isNegation)
                    AddNegationExpression(operatorAction, dynamicExpression.Body);
                else
                    AddNormalExpression(operatorAction, dynamicExpression.Body);
                break;
            }
            case LinqOperatorConstants.ConstantGreaterThanOrEqual:
            {
                dynamicExpression = DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}>=@0", castValue);
                if (isNegation)
                    AddNegationExpression(operatorAction, dynamicExpression.Body);
                else
                    AddNormalExpression(operatorAction, dynamicExpression.Body);
                break;
            }

            default:
                break;
        }
    }

    private void ComposeEqualsLinqExpression(string propertyName, OperatorType operatorAction,
        bool isNegation, object castValue, ParameterExpression x)
    {
        var dynamicExpression = DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}==@0", castValue);
        if (isNegation)
            AddNegationExpression(operatorAction, dynamicExpression.Body);
        else
            AddNormalExpression(operatorAction, dynamicExpression.Body);
    }

    /// <summary>
    /// Composes LINQ expression for date time based on <paramref name="extensionMethod"/>
    /// </summary>
    /// <param name="propertyName">Name of property to compose expression</param>
    /// <param name="extensionMethod">Extension method for which expression is composed</param>
    /// <param name="operatorAction">Operator action for expression</param>
    /// <param name="isNegation">Flag that indicates if expression should be negation</param>
    /// <param name="castValue">Casted value of <paramref name="propertyName"/></param>
    private void ComposeLambdaForDateTimeProperty(string propertyName, string extensionMethod, OperatorType operatorAction, bool isNegation, object castValue)
    {
        var dateTime = (DateTime)castValue;
        LambdaExpression dynamicExpression;
        var hoursDefined = dateTime.TimeOfDay.Hours > 0;
        var minutesDefined = dateTime.TimeOfDay.Minutes > 0;
        var secondsDefined = dateTime.TimeOfDay.Seconds > 0;
        var isTimeDefined = hoursDefined || minutesDefined || secondsDefined;
        switch (extensionMethod)
        {
            case LinqOperatorConstants.ConstantDateIs:
            {
                var x = _context.ParameterExpression;

                dynamicExpression = isTimeDefined ?
                     DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}==@0", dateTime)
                    : DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}>=@0 && {propertyName}<= @1", dateTime.Date, dateTime.Date.AddDays(1).AddTicks(-1));
                if (isNegation)
                    AddNegationExpression(operatorAction, dynamicExpression.Body);
                else
                    AddNormalExpression(operatorAction, dynamicExpression.Body);
                break;
            }
            case LinqOperatorConstants.ConstantBefore:
            {
                var x = _context.ParameterExpression;
                dynamicExpression = DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}<@0", dateTime);
                if (isNegation)
                    AddNegationExpression(operatorAction, dynamicExpression.Body);
                else
                    AddNormalExpression(operatorAction, dynamicExpression.Body);
                break;
            }
            case LinqOperatorConstants.ConstantAfter:
            {
                var x = _context.ParameterExpression;
                dynamicExpression = DynamicExpressionParser.ParseLambda([x], null, $"{propertyName}>@0", dateTime);
                if (isNegation)
                    AddNegationExpression(operatorAction, dynamicExpression.Body);
                else
                    AddNormalExpression(operatorAction, dynamicExpression.Body);
                break;
            }

            default:
                break;
        }
    }

    public IQueryable<TEntity> GetResult()
    {
        return _context.DataSet;
    }
}
