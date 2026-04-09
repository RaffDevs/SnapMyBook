using BookHighlights.Models;
using BookHighlights.Data;

namespace BookHighlights.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _db;

    public BooksController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IEnumerable<Book> GetAll() => _db.GetAllBooks();

    [HttpGet("{id}")]
    public ActionResult<Book> GetById(int id)
    {
        var book = _db.GetBookById(id);
        if (book == null) return NotFound();
        return book;
    }

    [HttpPost]
    public ActionResult<Book> Create([FromBody] Book book)
    {
        book.Id = _db.AddBook(book);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }
}
