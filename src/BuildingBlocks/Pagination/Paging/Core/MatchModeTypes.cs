namespace Paging.Core;

public static class MatchModeTypes
{
    public const string StartsWith = "startsWith";
    public const string Contains = "contains";
    public const string NotContains = "notContains";
    public const string EndsWith = "endsWith";
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public const string Equals = "equals";
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public const string NotEquals = "notEquals";
    public const string In = "in";
    public const string LessThan = "lt";
    public const string LessOrEqualsThan = "lte";
    public const string GreaterThan = "gt";
    public const string GreaterOrEqualsThan = "gte";
    public const string Between = "between";
    public const string Is = "is";
    public const string IsNot = "isNot";
    public const string Before = "before";
    public const string After = "after";
    public const string DateIs = "dateIs";
    public const string DateIsNot = "dateIsNot";
    public const string DateBefore = "dateBefore";
    public const string DateAfter = "dateAfter";
}
