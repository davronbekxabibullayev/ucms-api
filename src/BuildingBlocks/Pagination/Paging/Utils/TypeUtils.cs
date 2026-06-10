namespace Paging.Utils;

using System.Linq.Expressions;
using System.Reflection;

internal static class TypeUtils
{
    public static PropertyInfo? GetProperty(Type type, string propertyName)
    {
        var parts = propertyName.Split('.');

        if (parts.Length > 1)
        {
            var nestedProperty = type.GetProperty(parts[0])
                ?? throw new InvalidOperationException($"Property ${parts[0]} not does not exist.");

            return GetProperty(nestedProperty.PropertyType, parts.Skip(1).Aggregate((a, i) => a + "." + i));
        }

        return type.GetProperty(propertyName);
    }

    public static MemberExpression GetPropertyAccess(Type type, string propertyName, Expression expressionParam)
    {
        var parts = propertyName.Split('.');
        var access = expressionParam;
        var propertyType = type;

        foreach (var part in parts)
        {
            var prop = propertyType.GetProperty(part)
                ?? throw new InvalidOperationException($"Property ${part} not does not exist.");

            propertyType = prop.PropertyType;

            access = Expression.MakeMemberAccess(access, prop);
        }

        return (MemberExpression)access;
    }
}
