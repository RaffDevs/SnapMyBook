namespace BookHighlights.Models;

public class Highlight
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public int PageNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public Book? Book { get; set; }
}
