namespace SnapMyBook.Application.Books;

public interface IBookService
{
    Task<BookSummaryDto> CreateQuickAsync(QuickBookCreateRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookSummaryDto>> SearchAsync(string userId, string? search, CancellationToken cancellationToken = default);
    Task<BookDetailsDto?> GetDetailsAsync(Guid id, string userId, string? query, string? chapter, string? tag, CancellationToken cancellationToken = default);
}
