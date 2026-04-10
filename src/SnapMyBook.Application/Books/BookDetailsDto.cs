using SnapMyBook.Application.Highlights;

namespace SnapMyBook.Application.Books;

public sealed record BookDetailsDto(
    Guid Id,
    string Title,
    string? Author,
    string? Isbn,
    string? CoverUrl,
    int HighlightCount,
    IReadOnlyList<HighlightSummaryDto> Highlights);
