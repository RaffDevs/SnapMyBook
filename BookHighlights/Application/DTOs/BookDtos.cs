using BookHighlights.Domain.Entities;

namespace BookHighlights.Application.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public int HighlightsCount { get; set; }
}

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
}

public class HighlightDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public int BookId { get; set; }
    public string? BookTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateHighlightDto
{
    public string Text { get; set; } = string.Empty;
    public int BookId { get; set; }
    public string? Base64Image { get; set; }
}
