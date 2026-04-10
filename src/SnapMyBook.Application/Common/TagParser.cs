namespace SnapMyBook.Application.Common;

public static class TagParser
{
    public static IReadOnlyList<string> Parse(string? input)
    {
        return input?
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];
    }
}
