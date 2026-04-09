using BookHighlights.Models;
using BookHighlights.Data;

namespace BookHighlights.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HighlightsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public HighlightsController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet("book/{bookId}")]
    public IEnumerable<Highlight> GetByBook(int bookId) => _db.GetHighlightsByBookId(bookId);

    [HttpPost]
    public ActionResult<Highlight> Create([FromForm] HighlightCreateDto dto)
    {
        var book = _db.GetBookById(dto.BookId);
        if (book == null) return NotFound("Book not found");

        string? imagePath = null;
        if (dto.Image != null && dto.Image.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                dto.Image.CopyTo(stream);
            }

            imagePath = $"/uploads/{uniqueFileName}";
        }

        var highlight = new Highlight
        {
            BookId = dto.BookId,
            Text = dto.Text,
            ImagePath = imagePath,
            PageNumber = dto.PageNumber
        };

        highlight.Id = _db.AddHighlight(highlight);
        return CreatedAtAction(nameof(GetByBook), new { bookId = highlight.BookId }, highlight);
    }
}

public class HighlightCreateDto
{
    public int BookId { get; set; }
    public string Text { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public int PageNumber { get; set; }
}
