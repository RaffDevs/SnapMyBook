using Microsoft.EntityFrameworkCore;
using SnapMyBook.Application.Abstractions;
using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Infrastructure.Persistence;

public class BookRepository : IBookRepository
{
    private readonly SnapMyBookDbContext _dbContext;

    public BookRepository(SnapMyBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        return _dbContext.Books.AddAsync(book, cancellationToken).AsTask();
    }

    public Task<Book?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Books
            .Include(x => x.Highlights)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Book>> SearchAsync(string userId, string? search, CancellationToken cancellationToken = default)
    {
        IQueryable<Book> query = _dbContext.Books
            .Include(x => x.Highlights)
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim();
            query = query.Where(x => x.Title.Contains(normalized) || (x.Author != null && x.Author.Contains(normalized)));
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
