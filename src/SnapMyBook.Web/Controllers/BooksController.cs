using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapMyBook.Application.Books;
using SnapMyBook.Web.Models.Books;

namespace SnapMyBook.Web.Controllers;

[Authorize]
[Route("books")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, string? query, string? chapter, string? tag, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetDetailsAsync(id, UserId(), query, chapter, tag, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new QuickBookCreateViewModel { Step = 1 });
    }

    [HttpPost("create/basic")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateBasic(QuickBookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Step = 1;
            return View(model);
        }

        model.Step = 2;
        return View("CreateDetails", model);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuickBookCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Step = 2;
            return View("CreateDetails", model);
        }

        var book = await _bookService.CreateQuickAsync(
            new QuickBookCreateRequest(UserId(), model.Title, model.Author, model.Isbn, model.CoverUrl),
            cancellationToken);

        return RedirectToAction("Created", new { id = book.Id });
    }

    [HttpGet("{id:guid}/created")]
    public async Task<IActionResult> Created(Guid id, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetDetailsAsync(id, UserId(), null, null, null, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpGet("{id:guid}/highlights")]
    public async Task<IActionResult> Highlights(Guid id, string? query, string? chapter, string? tag, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetDetailsAsync(id, UserId(), query, chapter, tag, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        return PartialView("~/Views/Books/_HighlightsList.cshtml", book);
    }

    private string UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("User not authenticated.");
}
