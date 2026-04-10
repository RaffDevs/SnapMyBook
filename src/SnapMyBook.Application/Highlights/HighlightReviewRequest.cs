namespace SnapMyBook.Application.Highlights;

public sealed record HighlightReviewRequest(
    Guid BookId,
    string UserId,
    string RawText,
    string FinalText,
    string? Chapter,
    int? PageNumber,
    string? Base64Image,
    decimal? Confidence,
    string Language,
    bool OcrFailed,
    string? Tags);
