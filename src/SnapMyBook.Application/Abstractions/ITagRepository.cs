using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Application.Abstractions;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetOrCreateAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
}
