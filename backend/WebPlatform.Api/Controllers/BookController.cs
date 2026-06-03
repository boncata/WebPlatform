using Microsoft.AspNetCore.Mvc;
using WebPlatform.Api.Models;
using WebPlatform.Api.Services;

namespace WebPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;

    public BooksController(IBookService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var books = await _service.GetBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await _service.GetBookAsync(id);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook(Book book)
    {
        var created = await _service.AddBookAsync(book);

        // Automatically create the http request route, instead of using
        // something like $"/api/books/{created.Id}". This makes code more robust.
        return CreatedAtAction(nameof(GetBook), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
        if (id != book.Id)
            return BadRequest();

        var updated = await _service.UpdateBookAsync(book);

        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var deleted = await _service.DeleteBookAsync(id);

        // If the books was not found, return 404.
        if (!deleted)
            return NotFound();
        // Delete was successful, return 204 (No Content).
        return NoContent();
    }
}