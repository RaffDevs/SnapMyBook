using SnapMyBook.Application.Abstractions;
using SnapMyBook.Application.Common;
using SnapMyBook.Domain.Entities;
using SnapMyBook.Domain.Enums;

namespace SnapMyBook.Application.Highlights;

public class HighlightService : IHighlightService
{
    private readonly IBookRepository _bookRepository;
    private readonly IHighlightRepository _highlightRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public HighlightService(
        IBookRepository bookRepository,
        IHighlightRepository highlightRepository,
        ITagRepository tagRepository,
        IImageStorageService imageStorageService,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _highlightRepository = highlightRepository;
        _tagRepository = tagRepository;
        _imageStorageService = imageStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<HighlightSummaryDto> CreateAsync(HighlightReviewRequest request, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, request.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Book not found.");

        var imagePath = await _imageStorageService.SaveBase64ImageAsync(request.Base64Image, cancellationToken);
        var status = request.OcrFailed ? OcrStatus.Failed : OcrStatus.Processed;

        var highlight = new Highlight(
            book.Id,
            request.RawText,
            request.FinalText,
            request.Language,
            status,
            request.Chapter,
            request.PageNumber,
            imagePath,
            request.Confidence);

        var tags = await _tagRepository.GetOrCreateAsync(TagParser.Parse(request.Tags), cancellationToken);
        highlight.SetTags(tags);

        await _highlightRepository.AddAsync(highlight, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetAsync(highlight.Id, request.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load created highlight.");
    }

    public async Task<bool> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        var highlight = await _highlightRepository.GetByIdAsync(id, userId, cancellationToken);
        if (highlight is null)
        {
            return false;
        }

        await _highlightRepository.RemoveAsync(highlight, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<HighlightSummaryDto?> GetAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        var highlight = await _highlightRepository.GetByIdAsync(id, userId, cancellationToken);
        return highlight is null
            ? null
            : new HighlightSummaryDto(
                highlight.Id,
                highlight.BookId,
                highlight.FinalText,
                highlight.RawText,
                highlight.Chapter,
                highlight.PageNumber,
                highlight.ImagePath,
                highlight.Status.ToString(),
                highlight.Confidence,
                highlight.CapturedAtUtc,
                highlight.HighlightTags.Select(x => x.Tag?.Name ?? string.Empty).Where(name => !string.IsNullOrWhiteSpace(name)).ToList());
    }

    public async Task<HighlightSummaryDto?> UpdateAsync(HighlightEditRequest request, CancellationToken cancellationToken = default)
    {
        var highlight = await _highlightRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);
        if (highlight is null)
        {
            return null;
        }

        var tags = await _tagRepository.GetOrCreateAsync(TagParser.Parse(request.Tags), cancellationToken);
        highlight.UpdateReview(
            request.FinalText,
            request.RawText,
            request.Chapter,
            request.PageNumber,
            highlight.ImagePath,
            request.Confidence,
            request.Language,
            request.OcrFailed ? OcrStatus.Failed : OcrStatus.Processed);
        highlight.SetTags(tags);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetAsync(highlight.Id, request.UserId, cancellationToken);
    }
}
