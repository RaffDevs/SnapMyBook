using BookHighlights.Application.DTOs;
using BookHighlights.Domain.Entities;
using BookHighlights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookHighlights.Application.Services;

public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<BookDto> CreateBookAsync(CreateBookDto dto);
    Task DeleteBookAsync(int id);
}

public interface IHighlightService
{
    Task<List<HighlightDto>> GetHighlightsByBookIdAsync(int bookId);
    Task<HighlightDto> CreateHighlightAsync(CreateHighlightDto dto, string? imagePath);
    Task<HighlightDto?> GetHighlightByIdAsync(int id);
    Task DeleteHighlightAsync(int id);
}

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        var books = await _context.Books
            .Include(b => b.Highlights)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            CreatedAt = b.CreatedAt,
            HighlightsCount = b.Highlights.Count
        }).ToList();
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Highlights)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return null;

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CreatedAt = book.CreatedAt,
            HighlightsCount = book.Highlights.Count
        };
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
    {
        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CreatedAt = book.CreatedAt,
            HighlightsCount = 0
        };
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}

public class HighlightService : IHighlightService
{
    private readonly AppDbContext _context;

    public HighlightService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HighlightDto>> GetHighlightsByBookIdAsync(int bookId)
    {
        var highlights = await _context.Highlights
            .Where(h => h.BookId == bookId)
            .Include(h => h.Book)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

        return highlights.Select(h => new HighlightDto
        {
            Id = h.Id,
            Text = h.Text,
            ImagePath = h.ImagePath,
            BookId = h.BookId,
            BookTitle = h.Book?.Title,
            CreatedAt = h.CreatedAt
        }).ToList();
    }

    public async Task<HighlightDto> CreateHighlightAsync(CreateHighlightDto dto, string? imagePath)
    {
        var highlight = new Highlight
        {
            Text = dto.Text,
            BookId = dto.BookId,
            ImagePath = imagePath
        };

        _context.Highlights.Add(highlight);
        await _context.SaveChangesAsync();

        var book = await _context.Books.FindAsync(dto.BookId);

        return new HighlightDto
        {
            Id = highlight.Id,
            Text = highlight.Text,
            ImagePath = highlight.ImagePath,
            BookId = highlight.BookId,
            BookTitle = book?.Title,
            CreatedAt = highlight.CreatedAt
        };
    }

    public async Task<HighlightDto?> GetHighlightByIdAsync(int id)
    {
        var highlight = await _context.Highlights
            .Include(h => h.Book)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (highlight == null) return null;

        return new HighlightDto
        {
            Id = highlight.Id,
            Text = highlight.Text,
            ImagePath = highlight.ImagePath,
            BookId = highlight.BookId,
            BookTitle = highlight.Book?.Title,
            CreatedAt = highlight.CreatedAt
        };
    }

    public async Task DeleteHighlightAsync(int id)
    {
        var highlight = await _context.Highlights.FindAsync(id);
        if (highlight != null)
        {
            _context.Highlights.Remove(highlight);
            await _context.SaveChangesAsync();
        }
    }
}
