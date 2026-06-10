namespace Paging;
public enum SortingType
{
    OrderByAsc = 1,
    OrderByDesc = -1
}

public enum OperatorType
{
    And = 1,
    Or = 2,
    None = 3
}

public static class OperatorTypes
{
    private const string ConstantAnd = "and";
    private const string ConstantOr = "or";

    public static OperatorType ConvertToOperatorType(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            ConstantAnd => OperatorType.And,
            ConstantOr => OperatorType.Or,
            _ => OperatorType.None,
        };
    }
}
