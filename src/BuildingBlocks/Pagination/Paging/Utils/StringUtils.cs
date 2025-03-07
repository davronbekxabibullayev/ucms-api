namespace Paging.Utils;

public static class StringUtils
{
    public static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input.First().ToString().ToUpperInvariant(), input.AsSpan(1)),
        };
    }
}
