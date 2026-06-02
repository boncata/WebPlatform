// Book class for the books.

/*
Book class for the books.
*/
namespace WebPlatform.Api.Models;

public class Book
{
    public int Id {get; set;}
    public String? ISBN { get; set; }
    public String Title {get; set;} = "";
    public String Author {get; set;} = "";
    public String Description {get; set;} = "";
    public decimal Price {get; set;}
    public BookCondition Condition { get; set; }
}
