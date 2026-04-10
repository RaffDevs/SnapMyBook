namespace SnapMyBook.Application.Highlights;

public sealed record OcrResultDto(string RawText, decimal? Confidence, string Language, bool Failed, string? Error);
