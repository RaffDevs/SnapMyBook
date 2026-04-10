namespace SnapMyBook.Application.Books;

public sealed record QuickBookCreateRequest(string UserId, string Title, string? Author, string? Isbn, string? CoverUrl);
