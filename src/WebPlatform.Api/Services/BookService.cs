using WebPlatform.Api.Models;

namespace WebPlatform.Api.Services;

public class BookService: IBookService
{
    private readonly List<Book> _books = new();

    public BookService()
    {
        SeedBooks();
    }
    
    // Initialize some small database to begin with.
    private void SeedBooks()
    {
        _books.Add(new Book
        {
            Id = 0,
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Price = 30
        });

        _books.Add(new Book
        {
            Id = 1,
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt",
            Price = 28
        });

        _books.Add(new Book {
            Id=2,
            Title = "Shit the Can!",  
            Author="Dick Fanny",
            Price=19});

        _books.Add(new Book {
            Id=3,
            Title = "Top of the Morning!",
            Author="Fanny O'Cocker",
            Price=23});
    }

    public IEnumerable<Book> GetBooks()
    {
        return _books;
    }

    public Book? GetBook(int id)
    {
        return _books.FirstOrDefault(b => b.Id == id);
    }
    
    public Book AddBook(Book book)
    {
        // Give an ID to the new book by default
        // the max existing book ID + 1
        book.Id = _books.Max(b => b.Id) + 1;

        _books.Add(book);

        return book;
    }
}