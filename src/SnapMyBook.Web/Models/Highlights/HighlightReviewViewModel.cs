using System.ComponentModel.DataAnnotations;

namespace SnapMyBook.Web.Models.Highlights;

public class HighlightReviewViewModel
{
    public int Step { get; set; } = 2;
    public Guid BookId { get; set; }
    public Guid? HighlightId { get; set; }
    public string BookTitle { get; set; } = string.Empty;

    public string RawText { get; set; } = string.Empty;

    [Required]
    public string FinalText { get; set; } = string.Empty;

    public string? Chapter { get; set; }
    public int? PageNumber { get; set; }
    public string? Base64Image { get; set; }
    public string? StoredImagePath { get; set; }
    public decimal? Confidence { get; set; }
    public string Language { get; set; } = "por";
    public bool OcrFailed { get; set; }
    public string? Tags { get; set; }
}
