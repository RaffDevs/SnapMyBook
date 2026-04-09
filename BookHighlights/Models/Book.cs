namespace BookHighlights.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Highlight> Highlights { get; set; } = new();
}
