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
    public IActionResult GetBooks()
    {
        var books = _service.GetBooks();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public IActionResult GetBook(int id)
    {
        var book = _service.GetBook(id);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    public IActionResult CreateBook(Book book)
    {
        var created = _service.AddBook(book);

        // Automatically create the http request route, instead of using
        // something like $"/api/books/{created.Id}". This makes code more robust.
        return CreatedAtAction(nameof(GetBook), new { id = created.Id }, created);
    }
}