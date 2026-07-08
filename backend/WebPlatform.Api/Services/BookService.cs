using Microsoft.EntityFrameworkCore;
using WebPlatform.Api.Data;
using WebPlatform.Api.Dtos;
using WebPlatform.Api.Models;

namespace WebPlatform.Api.Services;

public class BookService: IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<PagedResult<Book>> GetBooksAsync(BookQueryParameters queryParams)
    {
        // Start with all books in the database. AsQueryable allows us to build a query
        // that can be executed against the database. This is important for performance,
        // as it allows us to only retrieve the data we need, rather than loading all books
        // into memory and then filtering them. This is especially important for large datasets.
        var books = _context.Books.AsQueryable();

        // Search functionality: if the Search parameter is not null or whitespace,
        // filter the books by Title or Author.
        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            books = books.Where(b =>
                b.Title.Contains(queryParams.Search) ||
                b.Author.Contains(queryParams.Search));
        }

        var totalCount = await books.CountAsync();

        var items = await books
            .OrderBy(b => b.Id)
            .Skip(queryParams.PageSize * (queryParams.Page - 1))
            .Take(queryParams.PageSize)
            .ToListAsync();

        return new PagedResult<Book>
        {
            Items = items,
            Page = queryParams.Page,
            PageSize = queryParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Book?> GetBookAsync(int id)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
    }
    
    public async Task<Book> AddBookAsync(BookRequest request)
    {
        // Map the BookRequest DTO to a Book instance.
        var book = new Book
        {
            ISBN = request.ISBN,
            Title = request.Title,
            Author = request.Author,
            PublicationYear = request.PublicationYear,
            Publisher = request.Publisher,
            Language = request.Language,
            Description = request.Description,
            Price = request.Price,
            Condition = request.Condition
        };

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

    public async Task<Book?> UpdateBookAsync(int id, BookRequest bookRequest)
    {
        var existingBook = await _context.Books.FindAsync(id);

        if (existingBook == null)
            // The book was not found, cannot update.
            return null;
        else
        {
            // Update the properties of the existing book with the new values.
            existingBook.ISBN = bookRequest.ISBN;
            existingBook.Title = bookRequest.Title;
            existingBook.Author = bookRequest.Author;
            existingBook.PublicationYear = bookRequest.PublicationYear;
            existingBook.Publisher = bookRequest.Publisher;
            existingBook.Language = bookRequest.Language;
            existingBook.Description = bookRequest.Description;
            existingBook.Price = bookRequest.Price;
            existingBook.Condition = bookRequest.Condition;

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