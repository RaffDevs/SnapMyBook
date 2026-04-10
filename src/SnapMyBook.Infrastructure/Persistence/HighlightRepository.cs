using Microsoft.EntityFrameworkCore;
using SnapMyBook.Application.Abstractions;
using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Infrastructure.Persistence;

public class HighlightRepository : IHighlightRepository
{
    private readonly SnapMyBookDbContext _dbContext;

    public HighlightRepository(SnapMyBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Highlight highlight, CancellationToken cancellationToken = default)
    {
        return _dbContext.Highlights.AddAsync(highlight, cancellationToken).AsTask();
    }

    public Task<Highlight?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Highlights
            .Include(x => x.Book)
            .Include(x => x.HighlightTags)
                .ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(x => x.Id == id && x.Book!.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Highlight>> GetByBookAsync(Guid bookId, string userId, string? query, string? chapter, string? tag, CancellationToken cancellationToken = default)
    {
        IQueryable<Highlight> highlights = _dbContext.Highlights
            .Include(x => x.HighlightTags)
                .ThenInclude(x => x.Tag)
            .Include(x => x.Book)
            .Where(x => x.BookId == bookId && x.Book!.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim();
            highlights = highlights.Where(x => x.FinalText.Contains(normalized) || x.RawText.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(chapter))
        {
            var normalizedChapter = chapter.Trim();
            highlights = highlights.Where(x => x.Chapter != null && x.Chapter.Contains(normalizedChapter));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var normalizedTag = tag.Trim().ToUpperInvariant();
            highlights = highlights.Where(x => x.HighlightTags.Any(t => t.Tag!.NormalizedName == normalizedTag));
        }

        return await highlights
            .OrderByDescending(x => x.CapturedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task RemoveAsync(Highlight highlight, CancellationToken cancellationToken = default)
    {
        _dbContext.Highlights.Remove(highlight);
        return Task.CompletedTask;
    }
}
