using SnapMyBook.Application.Common;
using SnapMyBook.Application.Highlights;

namespace SnapMyBook.Application.Tests.Highlights;

public class OcrWorkflowServiceTests
{
    [Fact]
    public void NormalizeClientResult_ShouldMarkEmptyTextAsFailure()
    {
        var service = new OcrWorkflowService();

        var result = service.NormalizeClientResult("   ", 87, "por", null);

        Assert.True(result.Failed);
        Assert.Equal("por", result.Language);
    }

    [Fact]
    public void TagParser_ShouldSplitAndDeduplicate()
    {
        var tags = TagParser.Parse("coragem, rotina, coragem");

        Assert.Equal(2, tags.Count);
        Assert.Contains(tags, tag => string.Equals(tag, "coragem", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(tags, tag => string.Equals(tag, "rotina", StringComparison.OrdinalIgnoreCase));
    }
}
