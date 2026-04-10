using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Application.Abstractions;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> SearchAsync(string userId, string? search, CancellationToken cancellationToken = default);
}
