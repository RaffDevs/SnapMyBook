using Microsoft.AspNetCore.Mvc;
using BookHighlights.Application.Services;
using BookHighlights.Application.DTOs;

namespace BookHighlights.Controllers;

public class HighlightsController : Controller
{
    private readonly IHighlightService _highlightService;
    private readonly IBookService _bookService;
    private readonly IWebHostEnvironment _environment;

    public HighlightsController(
        IHighlightService highlightService, 
        IBookService bookService,
        IWebHostEnvironment environment)
    {
        _highlightService = highlightService;
        _bookService = bookService;
        _environment = environment;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHighlightDto dto, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        string? imagePath = null;

        if (image != null && image.Length > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            imagePath = $"/uploads/{fileName}";
        }

        var highlight = await _highlightService.CreateHighlightAsync(dto, imagePath);
        
        return RedirectToAction(nameof(Books.Details), "Books", new { id = dto.BookId });
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Delete(int id, int bookId)
    {
        await _highlightService.DeleteHighlightAsync(id);
        return RedirectToAction(nameof(Books.Details), "Books", new { id = bookId });
    }
}
