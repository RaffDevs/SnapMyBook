using SnapMyBook.Domain.Enums;

namespace SnapMyBook.Domain.Entities;

public class Highlight
{
    private readonly List<HighlightTag> _highlightTags = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public string RawText { get; private set; } = string.Empty;
    public string FinalText { get; private set; } = string.Empty;
    public string? Chapter { get; private set; }
    public int? PageNumber { get; private set; }
    public string? ImagePath { get; private set; }
    public decimal? Confidence { get; private set; }
    public string Language { get; private set; } = "por";
    public OcrStatus Status { get; private set; } = OcrStatus.Pending;
    public DateTime CapturedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public Book? Book { get; private set; }
    public IReadOnlyCollection<HighlightTag> HighlightTags => _highlightTags;

    private Highlight()
    {
    }

    public Highlight(
        Guid bookId,
        string rawText,
        string finalText,
        string language,
        OcrStatus status,
        string? chapter = null,
        int? pageNumber = null,
        string? imagePath = null,
        decimal? confidence = null)
    {
        if (bookId == Guid.Empty)
        {
            throw new ArgumentException("Book is required.", nameof(bookId));
        }

        if (string.IsNullOrWhiteSpace(finalText))
        {
            throw new ArgumentException("Highlight text is required.", nameof(finalText));
        }

        BookId = bookId;
        RawText = rawText.Trim();
        FinalText = finalText.Trim();
        Language = string.IsNullOrWhiteSpace(language) ? "por" : language.Trim();
        Status = status;
        Chapter = chapter?.Trim();
        PageNumber = pageNumber;
        ImagePath = imagePath;
        Confidence = confidence;
    }

    public void UpdateReview(
        string finalText,
        string? rawText,
        string? chapter,
        int? pageNumber,
        string? imagePath,
        decimal? confidence,
        string language,
        OcrStatus status)
    {
        if (string.IsNullOrWhiteSpace(finalText))
        {
            throw new ArgumentException("Highlight text is required.", nameof(finalText));
        }

        FinalText = finalText.Trim();
        RawText = rawText?.Trim() ?? string.Empty;
        Chapter = chapter?.Trim();
        PageNumber = pageNumber;
        ImagePath = imagePath;
        Confidence = confidence;
        Language = string.IsNullOrWhiteSpace(language) ? "por" : language.Trim();
        Status = status;
    }

    public void SetTags(IEnumerable<Tag> tags)
    {
        _highlightTags.Clear();

        foreach (var tag in tags.DistinctBy(x => x.Id))
        {
            _highlightTags.Add(new HighlightTag(Id, tag.Id));
        }
    }
}
