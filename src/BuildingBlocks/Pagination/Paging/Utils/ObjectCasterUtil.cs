namespace Paging.Utils;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Nodes;
using Paging.Models;

public static class ObjectCasterUtil
{
    public static object CastPropertiesTypeList(PropertyInfo property, object value)
    {
        var arrayCast = (JsonArray)value;

        if (property?.PropertyType == typeof(int))
            return arrayCast.GetValue<List<int>>();
        if (property?.PropertyType == typeof(int?))
            return arrayCast.GetValue<List<int?>>();
        if (property?.PropertyType == typeof(double))
            return arrayCast.GetValue<List<double>>();
        if (property?.PropertyType == typeof(double?))
            return arrayCast.GetValue<List<double?>>();
        if (property?.PropertyType == typeof(DateTime))
            return arrayCast.GetValue<List<DateTime>>();
        if (property?.PropertyType == typeof(DateTime?))
            return arrayCast.GetValue<List<DateTime?>>();
        if (property?.PropertyType == typeof(bool))
            return arrayCast.GetValue<List<bool>>();
        if (property?.PropertyType == typeof(bool?))
            return arrayCast.GetValue<List<bool?>>();
        if (property?.PropertyType == typeof(short))
            return arrayCast.GetValue<List<short>>();
        if (property?.PropertyType == typeof(short?))
            return arrayCast.GetValue<List<short?>>();
        if (property?.PropertyType == typeof(long))
            return arrayCast.GetValue<List<long>>();
        if (property?.PropertyType == typeof(long?))
            return arrayCast.GetValue<List<long?>>();
        if (property?.PropertyType == typeof(float))
            return arrayCast.GetValue<List<float>>();
        if (property?.PropertyType == typeof(float?))
            return arrayCast.GetValue<List<float?>>();
        if (property?.PropertyType == typeof(decimal))
            return arrayCast.GetValue<List<decimal>>();
        if (property?.PropertyType == typeof(decimal?))
            return arrayCast.GetValue<List<decimal?>>();

        return arrayCast.GetValue<List<string>>();
    }

    public static object CastPropertiesType(PropertyInfo property, object value)
    {
        if (property?.PropertyType == typeof(int))
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(int?))
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType.IsEnum ?? false)
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(double))
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(double?))
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(DateTime))
            return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(DateTime?))
            return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(bool))
            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(bool?))
            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(short))
            return Convert.ToInt16(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(short?))
            return Convert.ToInt16(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(long))
            return Convert.ToInt64(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(long?))
            return Convert.ToInt64(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(float))
            return Convert.ToSingle(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(float?))
            return Convert.ToSingle(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(decimal))
            return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(decimal?))
            return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        if (property?.PropertyType == typeof(Guid))
            return value == null ? Guid.Empty : Guid.Parse(value.ToString()!);
        if (property?.PropertyType == typeof(Guid?))
            return value == null ? Guid.Empty : Guid.Parse(value.ToString()!);


        return value?.ToString() ?? string.Empty;
    }

    public static FilterMeta CastJObjectToTableFilterContext(JsonObject obj)
    {
        return obj.GetValue<FilterMeta>();
    }
}
