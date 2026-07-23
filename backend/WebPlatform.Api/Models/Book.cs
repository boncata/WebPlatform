// Book class for the books.

/*
Book class for the books.
*/
namespace WebPlatform.Api.Models;

public class Book
{
    public int Id {get; set;}
    public string? ISBN { get; set; }
    public string Title {get; set;} = "";
    public string Author {get; set;} = "";
    public int? PublicationYear { get; set; }
    public string Publisher { get; set; } = "";
    public string Language { get; set; } = "";
    public string Description {get; set;} = "";
    public decimal Price {get; set;}
    public BookCondition Condition { get; set; }
}
