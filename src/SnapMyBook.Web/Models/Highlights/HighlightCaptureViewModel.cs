using System.ComponentModel.DataAnnotations;

namespace SnapMyBook.Web.Models.Highlights;

public class HighlightCaptureViewModel
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string? BookAuthor { get; set; }

    [StringLength(160)]
    public string? Chapter { get; set; }

    [Range(1, 5000)]
    public int? PageNumber { get; set; }

    public string? Tags { get; set; }
}
