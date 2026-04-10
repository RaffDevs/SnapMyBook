namespace SnapMyBook.Application.Highlights;

public sealed record HighlightEditRequest(
    Guid Id,
    string UserId,
    string FinalText,
    string RawText,
    string? Chapter,
    int? PageNumber,
    decimal? Confidence,
    string Language,
    bool OcrFailed,
    string? Tags);
