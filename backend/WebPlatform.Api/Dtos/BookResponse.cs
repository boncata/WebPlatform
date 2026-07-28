using WebPlatform.Api.Models;

namespace WebPlatform.Api.Dtos;

/**
 * DTO (Data Transfer Object) class, used to shape the data sent back to
 * the client when returning a book. Keeping this separate from the Book
 * entity means the database model (e.g. new EF navigation properties)
 * can evolve without automatically changing the public API contract.
**/

public class BookResponse
{
    public int Id { get; set; }
    public string? ISBN { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int? PublicationYear { get; set; }
    public string Publisher { get; set; } = "";
    public string Language { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public BookCondition Condition { get; set; }
}
