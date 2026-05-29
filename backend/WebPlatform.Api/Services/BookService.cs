using WebPlatform.Api.Data;
using WebPlatform.Api.Models;

namespace WebPlatform.Api.Services;

public class BookService: IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Book> GetBooks()
    {
        return _context.Books.ToList();
    }

    public Book? GetBook(int id)
    {
        return _context.Books.FirstOrDefault(b => b.Id == id);
    }
    
    public Book AddBook(Book book)
    {
        // This only "schedules" the add. The book Id is automatically set by PostgreSQL.
        _context.Books.Add(book);
        // This actually makes the changes to the database.
        _context.SaveChanges();

        return book;
    }

    public bool DeleteBook(int id)
    {
        var book = _context.Books.Find(id);

        if (book == null)
            // The book was not found, cannot delete.
            return false;
        else
        {
            // Schedule the delete, then save the changes to the database.
            _context.Books.Remove(book);
            _context.SaveChanges();
            // Return true, as the delete was successful.
            return true;
        }

    }
}