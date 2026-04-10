namespace SnapMyBook.Application.Books;

public sealed record BookSummaryDto(
    Guid Id,
    string Title,
    string? Author,
    string? CoverUrl,
    int HighlightCount,
    DateTime CreatedAtUtc);
