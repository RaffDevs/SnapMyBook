using System.ComponentModel.DataAnnotations;

namespace SnapMyBook.Web.Models.Books;

public class QuickBookCreateViewModel
{
    public int Step { get; set; } = 1;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(160)]
    public string? Author { get; set; }

    [StringLength(32)]
    public string? Isbn { get; set; }

    [Url]
    public string? CoverUrl { get; set; }

    public string? Search { get; set; }
}
