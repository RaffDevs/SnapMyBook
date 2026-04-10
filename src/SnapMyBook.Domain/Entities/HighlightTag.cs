namespace SnapMyBook.Domain.Entities;

public class HighlightTag
{
    public Guid HighlightId { get; private set; }
    public Guid TagId { get; private set; }

    public Highlight? Highlight { get; private set; }
    public Tag? Tag { get; private set; }

    private HighlightTag()
    {
    }

    public HighlightTag(Guid highlightId, Guid tagId)
    {
        HighlightId = highlightId;
        TagId = tagId;
    }
}
