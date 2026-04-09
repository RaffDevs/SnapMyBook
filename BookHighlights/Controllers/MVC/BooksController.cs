using BookHighlights.Models;
using BookHighlights.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookHighlights.Controllers.MVC;

public class BooksController : Controller
{
    private readonly AppDbContext _db;

    public BooksController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var books = _db.GetAllBooks();
        return View(books);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public IActionResult Create(Book book)
    {
        if (ModelState.IsValid)
        {
            book.Id = _db.AddBook(book);
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    [HttpGet("{id}")]
    public IActionResult Details(int id)
    {
        var book = _db.GetBookById(id);
        if (book == null) return NotFound();
        
        var highlights = _db.GetHighlightsByBookId(id);
        ViewBag.Highlights = highlights;
        
        return View(book);
    }

    [HttpGet("{id}/capture")]
    public IActionResult Capture(int id)
    {
        var book = _db.GetBookById(id);
        if (book == null) return NotFound();
        
        ViewBag.BookId = id;
        ViewBag.BookTitle = book.Title;
        
        return View();
    }
}
