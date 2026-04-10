using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Domain.Tests.Entities;

public class BookTests
{
    [Fact]
    public void Create_ShouldRequireTitle()
    {
        var action = () => new Book("user-1", "");

        var exception = Assert.Throws<ArgumentException>(action);

        Assert.True(string.Equals("title", exception.ParamName, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Highlight_ShouldReplaceTagsWithoutDuplicates()
    {
        var highlight = new Highlight(Guid.NewGuid(), "raw", "final", "por", SnapMyBook.Domain.Enums.OcrStatus.Processed);
        var courage = new Tag("Coragem");
        var routine = new Tag("Rotina");

        highlight.SetTags([courage, courage, routine]);

        Assert.Equal(2, highlight.HighlightTags.Count);
    }
}
