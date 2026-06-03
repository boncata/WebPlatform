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

    public Book? UpdateBook(Book book)
    {
        var existingBook = _context.Books.Find(book.Id);

        if (existingBook == null)
            // The book was not found, cannot update.
            return null;
        else
        {
            // Update the properties of the existing book with the new values.
            existingBook.ISBN = book.ISBN;
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Description = book.Description;
            existingBook.Price = book.Price;
            existingBook.Condition = book.Condition;

            // Save the changes to the database.
            _context.SaveChanges();

            return existingBook;
        }
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