using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapMyBook.Application.Books;
using SnapMyBook.Application.Highlights;
using SnapMyBook.Web.Models.Highlights;

namespace SnapMyBook.Web.Controllers;

[Authorize]
[Route("highlights")]
public class HighlightsController : Controller
{
    private readonly IBookService _bookService;
    private readonly IHighlightService _highlightService;
    private readonly IOcrWorkflowService _ocrWorkflowService;

    public HighlightsController(IBookService bookService, IHighlightService highlightService, IOcrWorkflowService ocrWorkflowService)
    {
        _bookService = bookService;
        _highlightService = highlightService;
        _ocrWorkflowService = ocrWorkflowService;
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(Guid bookId, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetDetailsAsync(bookId, UserId(), null, null, null, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        return View(new HighlightCaptureViewModel
        {
            BookId = book.Id,
            BookTitle = book.Title,
            BookAuthor = book.Author
        });
    }

    [HttpPost("review")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(HighlightReviewViewModel model, string? errorMessage, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetDetailsAsync(model.BookId, UserId(), null, null, null, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        var ocr = _ocrWorkflowService.NormalizeClientResult(model.RawText, model.Confidence, model.Language, errorMessage);
        model.BookTitle = book.Title;
        model.Step = 2;
        model.RawText = ocr.RawText;
        model.Language = ocr.Language;
        model.Confidence = ocr.Confidence;
        model.OcrFailed = ocr.Failed;

        if (string.IsNullOrWhiteSpace(model.FinalText))
        {
            model.FinalText = ocr.RawText;
        }

        return View("Review", model);
    }

    [HttpPost("metadata")]
    [ValidateAntiForgeryToken]
    public IActionResult Metadata(HighlightReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Step = 2;
            return View("Review", model);
        }

        model.Step = 3;
        return View("Metadata", model);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HighlightReviewViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Step = 3;
            return View("Metadata", model);
        }

        await _highlightService.CreateAsync(
            new HighlightReviewRequest(
                model.BookId,
                UserId(),
                model.RawText,
                model.FinalText,
                model.Chapter,
                model.PageNumber,
                model.Base64Image,
                model.Confidence,
                model.Language,
                model.OcrFailed,
                model.Tags),
            cancellationToken);

        return RedirectToAction("Details", "Books", new { id = model.BookId });
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var highlight = await _highlightService.GetAsync(id, UserId(), cancellationToken);
        if (highlight is null)
        {
            return NotFound();
        }

        return View(new HighlightReviewViewModel
        {
            BookId = highlight.BookId,
            HighlightId = highlight.Id,
            FinalText = highlight.FinalText,
            RawText = highlight.RawText,
            Chapter = highlight.Chapter,
            PageNumber = highlight.PageNumber,
            StoredImagePath = highlight.ImagePath,
            Confidence = highlight.Confidence,
            OcrFailed = string.Equals(highlight.Status, "Failed", StringComparison.OrdinalIgnoreCase),
            Tags = string.Join(", ", highlight.Tags)
        });
    }

    [HttpPost("{id:guid}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, HighlightReviewViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _highlightService.UpdateAsync(
            new HighlightEditRequest(
                id,
                UserId(),
                model.FinalText,
                model.RawText,
                model.Chapter,
                model.PageNumber,
                model.Confidence,
                model.Language,
                model.OcrFailed,
                model.Tags),
            cancellationToken);

        if (updated is null)
        {
            return NotFound();
        }

        return RedirectToAction("Details", "Books", new { id = model.BookId });
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid bookId, CancellationToken cancellationToken)
    {
        await _highlightService.DeleteAsync(id, UserId(), cancellationToken);
        return RedirectToAction("Details", "Books", new { id = bookId });
    }

    private string UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("User not authenticated.");
}
