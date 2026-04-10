namespace SnapMyBook.Application.Highlights;

public interface IHighlightService
{
    Task<HighlightSummaryDto> CreateAsync(HighlightReviewRequest request, CancellationToken cancellationToken = default);
    Task<HighlightSummaryDto?> GetAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<HighlightSummaryDto?> UpdateAsync(HighlightEditRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string userId, CancellationToken cancellationToken = default);
}
