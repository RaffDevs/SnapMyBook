namespace BookHighlights.Domain.Entities;

public class Highlight
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
