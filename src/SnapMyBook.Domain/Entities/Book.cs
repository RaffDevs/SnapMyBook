namespace SnapMyBook.Domain.Entities;

public class Book
{
    private readonly List<Highlight> _highlights = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserId { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Author { get; private set; }
    public string? Isbn { get; private set; }
    public string? CoverUrl { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<Highlight> Highlights => _highlights;

    private Book()
    {
    }

    public Book(string userId, string title, string? author = null, string? isbn = null, string? coverUrl = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        UserId = userId.Trim();
        Title = title.Trim();
        Author = author?.Trim();
        Isbn = isbn?.Trim();
        CoverUrl = coverUrl?.Trim();
    }

    public void UpdateMetadata(string title, string? author, string? isbn, string? coverUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        Title = title.Trim();
        Author = author?.Trim();
        Isbn = isbn?.Trim();
        CoverUrl = coverUrl?.Trim();
    }
}
