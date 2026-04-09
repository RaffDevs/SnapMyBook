using BookHighlights.Domain.Entities;
using BookHighlights.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookHighlights.Infrastructure.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateAsync(Book book);
    Task DeleteAsync(int id);
}

public interface IHighlightRepository
{
    Task<List<Highlight>> GetByBookIdAsync(int bookId);
    Task<Highlight> CreateAsync(Highlight highlight);
    Task<Highlight?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _context.Books
            .Include(b => b.Highlights)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Highlights)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}

public class HighlightRepository : IHighlightRepository
{
    private readonly AppDbContext _context;

    public HighlightRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Highlight>> GetByBookIdAsync(int bookId)
    {
        return await _context.Highlights
            .Where(h => h.BookId == bookId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<Highlight> CreateAsync(Highlight highlight)
    {
        _context.Highlights.Add(highlight);
        await _context.SaveChangesAsync();
        return highlight;
    }

    public async Task<Highlight?> GetByIdAsync(int id)
    {
        return await _context.Highlights
            .Include(h => h.Book)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task DeleteAsync(int id)
    {
        var highlight = await _context.Highlights.FindAsync(id);
        if (highlight != null)
        {
            _context.Highlights.Remove(highlight);
            await _context.SaveChangesAsync();
        }
    }
}
