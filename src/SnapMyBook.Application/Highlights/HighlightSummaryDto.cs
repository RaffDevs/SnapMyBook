namespace SnapMyBook.Application.Highlights;

public sealed record HighlightSummaryDto(
    Guid Id,
    Guid BookId,
    string FinalText,
    string RawText,
    string? Chapter,
    int? PageNumber,
    string? ImagePath,
    string Status,
    decimal? Confidence,
    DateTime CapturedAtUtc,
    IReadOnlyList<string> Tags);
