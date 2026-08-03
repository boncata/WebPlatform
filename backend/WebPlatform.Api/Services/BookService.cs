using System.Linq.Expressions;
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
    
    public async Task<PagedResult<BookResponse>> GetBooksAsync(BookQueryParameters queryParams)
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

        // Language filter: case-insensitive, since users may type the
        // language name in any casing (e.g. "english" vs "English").
        if (!string.IsNullOrWhiteSpace(queryParams.Language))
        {
            books = books.Where(b => b.Language.ToLower() == queryParams.Language.ToLower());
        }

        // Condition filter: exact match against the requested enum value.
        if (queryParams.Condition.HasValue)
        {
            books = books.Where(b => b.Condition == queryParams.Condition.Value);
        }

        // Max price filter: only return books at or below the given price.
        if (queryParams.MaxPrice.HasValue)
        {
            books = books.Where(b => b.Price <= queryParams.MaxPrice.Value);
        }

        // ISBN filter: exact match, since an ISBN identifies a specific book.
        if (!string.IsNullOrWhiteSpace(queryParams.ISBN))
        {
            books = books.Where(b => b.ISBN == queryParams.ISBN);
        }

        // Publisher filter: partial match, since publisher names are free
        // text (e.g. "Wesley" should find "Addison-Wesley").
        if (!string.IsNullOrWhiteSpace(queryParams.Publisher))
        {
            books = books.Where(b => b.Publisher.Contains(queryParams.Publisher));
        }

        // Publication year filter: exact match against a single year.
        if (queryParams.PublicationYear.HasValue)
        {
            books = books.Where(b => b.PublicationYear == queryParams.PublicationYear.Value);
        }

        var totalCount = await books.CountAsync();

        // Sort by the requested field, or fall back to Id order when no
        // SortBy was given.
        var orderedBooks = queryParams.SortBy switch
        {
            BookSortField.Title => ApplySortOrder(books, b => b.Title, queryParams.SortOrder),
            BookSortField.Price => ApplySortOrder(books, b => b.Price, queryParams.SortOrder),
            BookSortField.PublicationYear => ApplySortOrder(books, b => b.PublicationYear, queryParams.SortOrder),
            _ => books.OrderBy(b => b.Id)
        };

        var items = await orderedBooks
            // Id as a secondary sort key guarantees a stable, deterministic
            // order across pages even when many books share the same value
            // for the primary sort field (e.g. the same Price).
            .ThenBy(b => b.Id)
            .Skip(queryParams.PageSize * (queryParams.Page - 1))
            .Take(queryParams.PageSize)
            .ToListAsync();

        return new PagedResult<BookResponse>
        {
            Items = items.Select(ToResponse).ToList(),
            Page = queryParams.Page,
            PageSize = queryParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BookResponse?> GetBookAsync(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        return book == null ? null : ToResponse(book);
    }

    public async Task<BookResponse> AddBookAsync(BookRequest request)
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

        return ToResponse(book);
    }

    public async Task<BookResponse?> UpdateBookAsync(int id, BookRequest bookRequest)
    {
        var existingBook = await _context.Books.FindAsync(id);

        if (existingBook == null)
            // The book was not found, cannot update.
            return null;

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

        return ToResponse(existingBook);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
            // The book was not found, cannot delete.
            return false;

        // Schedule the delete, then save the changes to the database.
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        // Return true, as the delete was successful.
        return true;
    }

    /// <summary>
    /// Applies ascending or descending ordering to <paramref name="books"/> based on
    /// <paramref name="keySelector"/>. Returns <see cref="IOrderedQueryable{T}"/>
    /// (not <see cref="IQueryable{T}"/>) so callers can chain <c>.ThenBy(...)</c>
    /// afterwards, e.g. to add a tie-breaking sort key.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the property being sorted on (e.g. <see cref="string"/> for Title,
    /// <see cref="decimal"/> for Price). Inferred automatically from
    /// <paramref name="keySelector"/> at the call site, which is why one generic
    /// method can replace a separate ascending/descending check per field.
    /// </typeparam>
    /// <param name="books">The query to sort, after any filtering has already been applied.</param>
    /// <param name="keySelector">
    /// The property to sort by, as an expression tree rather than a compiled delegate,
    /// so EF Core can translate it into a SQL ORDER BY clause instead of pulling every
    /// row into memory and sorting there.
    /// </param>
    /// <param name="sortOrder">Whether to sort ascending or descending.</param>
    /// <returns>The books query with the requested ordering applied.</returns>
    private static IOrderedQueryable<Book> ApplySortOrder<TKey>(
        IQueryable<Book> books, Expression<Func<Book, TKey>> keySelector, SortDirection sortOrder)
    {
        return sortOrder == SortDirection.Descending
            ? books.OrderByDescending(keySelector)
            : books.OrderBy(keySelector);
    }

    // Maps the EF entity to the DTO returned by the API, so callers of
    // this service never see the database model directly.
    private static BookResponse ToResponse(Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Author = book.Author,
            PublicationYear = book.PublicationYear,
            Publisher = book.Publisher,
            Language = book.Language,
            Description = book.Description,
            Price = book.Price,
            Condition = book.Condition
        };
    }
}