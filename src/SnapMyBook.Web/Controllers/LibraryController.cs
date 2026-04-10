using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapMyBook.Application.Books;
using SnapMyBook.Web.Models.Library;

namespace SnapMyBook.Web.Controllers;

[Authorize]
[Route("library")]
public class LibraryController : Controller
{
    private readonly IBookService _bookService;

    public LibraryController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, CancellationToken cancellationToken)
    {
        var books = await _bookService.SearchAsync(UserId(), search, cancellationToken);
        return View(new LibraryIndexViewModel
        {
            Search = search,
            Books = books,
            QuickCreate = new() { Search = search }
        });
    }

    [HttpGet("books-grid")]
    public async Task<IActionResult> BooksGrid(string? search, CancellationToken cancellationToken)
    {
        var books = await _bookService.SearchAsync(UserId(), search, cancellationToken);
        return PartialView("~/Views/Library/_BooksGrid.cshtml", books);
    }

    [AllowAnonymous]
    [HttpGet("/")]
    public IActionResult Root()
    {
        return User.Identity?.IsAuthenticated == true
            ? RedirectToAction(nameof(Index))
            : RedirectToAction("Login", "Auth");
    }

    [AllowAnonymous]
    [HttpGet("error")]
    public IActionResult Error()
    {
        return View("~/Views/Shared/Error.cshtml");
    }

    private string UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("User not authenticated.");
}
