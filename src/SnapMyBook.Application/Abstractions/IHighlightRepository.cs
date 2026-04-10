using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Application.Abstractions;

public interface IHighlightRepository
{
    Task AddAsync(Highlight highlight, CancellationToken cancellationToken = default);
    Task<Highlight?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Highlight>> GetByBookAsync(Guid bookId, string userId, string? query, string? chapter, string? tag, CancellationToken cancellationToken = default);
    Task RemoveAsync(Highlight highlight, CancellationToken cancellationToken = default);
}
