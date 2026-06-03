using Microsoft.EntityFrameworkCore;
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
    
    public async Task<IEnumerable<Book>> GetBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<Book?> GetBookAsync(int id)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
    }
    
    public async Task<Book> AddBookAsync(Book book)
    {
        // This only "schedules" the add. The book Id is automatically
        // set by PostgreSQL. For EF Core, adding the book does not
        // actually make the changes to the database, until we call
        // SaveChanges. Therefore, we can keep Add to be synchronous,
        // and make the method asynchronous by making SaveChangesAsync
        // asynchronous.
        _context.Books.Add(book);
        // This actually makes the changes to the database.
        await _context.SaveChangesAsync();

        return book;
    }

    public async Task<Book?> UpdateBookAsync(Book book)
    {
        var existingBook = await _context.Books.FindAsync(book.Id);

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
            await _context.SaveChangesAsync();

            return existingBook;
        }
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
            // The book was not found, cannot delete.
            return false;
        else
        {
            // Schedule the delete, then save the changes to the database.
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            // Return true, as the delete was successful.
            return true;
        }

    }
}