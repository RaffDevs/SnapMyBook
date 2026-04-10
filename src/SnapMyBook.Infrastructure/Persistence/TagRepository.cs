using Microsoft.EntityFrameworkCore;
using SnapMyBook.Application.Abstractions;
using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Infrastructure.Persistence;

public class TagRepository : ITagRepository
{
    private readonly SnapMyBookDbContext _dbContext;

    public TagRepository(SnapMyBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Tag>> GetOrCreateAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        var distinctNames = names
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (distinctNames.Count == 0)
        {
            return [];
        }

        var normalized = distinctNames.Select(x => x.ToUpperInvariant()).ToList();
        var existing = await _dbContext.Tags
            .Where(x => normalized.Contains(x.NormalizedName))
            .ToListAsync(cancellationToken);

        var missing = distinctNames
            .Where(x => existing.All(e => !string.Equals(e.Name, x, StringComparison.OrdinalIgnoreCase)))
            .Select(x => new Tag(x))
            .ToList();

        if (missing.Count > 0)
        {
            await _dbContext.Tags.AddRangeAsync(missing, cancellationToken);
        }

        return existing.Concat(missing).ToList();
    }
}
