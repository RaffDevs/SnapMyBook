using SnapMyBook.Application.Books;
using SnapMyBook.Web.Models.Books;

namespace SnapMyBook.Web.Models.Library;

public class LibraryIndexViewModel
{
    public string? Search { get; set; }
    public IReadOnlyList<BookSummaryDto> Books { get; set; } = [];
    public QuickBookCreateViewModel QuickCreate { get; set; } = new();
}
