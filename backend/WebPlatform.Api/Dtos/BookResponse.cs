using WebPlatform.Api.Models;

namespace WebPlatform.Api.Dtos;

/**
 * DTO (Data Transfer Object) class, used to shape the data sent back to
 * the client when returning a book. Keeping this separate from the Book
 * entity means the database model (e.g. new EF navigation properties)
 * can evolve without automatically changing the public API contract.
**/

public record BookResponse
{
    public int Id { get; init; }
    public string? ISBN { get; init; }
    public string Title { get; init; } = "";
    public string Author { get; init; } = "";
    public int? PublicationYear { get; init; }
    public string Publisher { get; init; } = "";
    public string Language { get; init; } = "";
    public string Description { get; init; } = "";
    public decimal Price { get; init; }
    public BookCondition Condition { get; init; }
}
