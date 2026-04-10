using SnapMyBook.Application.Abstractions;
using SnapMyBook.Application.Highlights;
using SnapMyBook.Domain.Entities;

namespace SnapMyBook.Application.Books;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IHighlightRepository _highlightRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IBookRepository bookRepository, IHighlightRepository highlightRepository, IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _highlightRepository = highlightRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookSummaryDto> CreateQuickAsync(QuickBookCreateRequest request, CancellationToken cancellationToken = default)
    {
        var book = new Book(request.UserId, request.Title, request.Author, request.Isbn, request.CoverUrl);

        await _bookRepository.AddAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BookSummaryDto(book.Id, book.Title, book.Author, book.CoverUrl, 0, book.CreatedAtUtc);
    }

    public async Task<BookDetailsDto?> GetDetailsAsync(Guid id, string userId, string? query, string? chapter, string? tag, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, userId, cancellationToken);
        if (book is null)
        {
            return null;
        }

        var highlights = await _highlightRepository.GetByBookAsync(id, userId, query, chapter, tag, cancellationToken);
        var highlightDtos = highlights.Select(x =>
            new HighlightSummaryDto(
                x.Id,
                x.BookId,
                x.FinalText,
                x.RawText,
                x.Chapter,
                x.PageNumber,
                x.ImagePath,
                x.Status.ToString(),
                x.Confidence,
                x.CapturedAtUtc,
                x.HighlightTags.Select(t => t.Tag?.Name ?? string.Empty).Where(name => !string.IsNullOrWhiteSpace(name)).ToList()))
            .ToList();

        return new BookDetailsDto(book.Id, book.Title, book.Author, book.Isbn, book.CoverUrl, highlightDtos.Count, highlightDtos);
    }

    public async Task<IReadOnlyList<BookSummaryDto>> SearchAsync(string userId, string? search, CancellationToken cancellationToken = default)
    {
        var books = await _bookRepository.SearchAsync(userId, search, cancellationToken);
        return books
            .Select(x => new BookSummaryDto(x.Id, x.Title, x.Author, x.CoverUrl, x.Highlights.Count, x.CreatedAtUtc))
            .ToList();
    }
}
