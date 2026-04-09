using Microsoft.AspNetCore.Mvc;
using BookHighlights.Application.Services;
using BookHighlights.Application.DTOs;

namespace BookHighlights.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IHighlightService _highlightService;

    public BooksController(IBookService bookService, IHighlightService highlightService)
    {
        _bookService = bookService;
        _highlightService = highlightService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllBooksAsync();
        return View(books);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var book = await _bookService.CreateBookAsync(dto);
        return RedirectToAction(nameof(Details), new { id = book.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        var highlights = await _highlightService.GetHighlightsByBookIdAsync(id);
        ViewBag.Highlights = highlights;
        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _bookService.DeleteBookAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
