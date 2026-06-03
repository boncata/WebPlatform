using WebPlatform.Api.Dtos;
using WebPlatform.Api.Models;

namespace WebPlatform.Api.Services;

// Interface for our books service. Must be implemented by
// any and all book services that we have.
public interface IBookService
{
    Task<IEnumerable<Book>> GetBooksAsync();

    Task<Book?> GetBookAsync(int id);

    Task<Book> AddBookAsync(BookRequest request);

    Task<Book?> UpdateBookAsync(int id, BookRequest bookRequest);

    Task<bool> DeleteBookAsync(int id);
}
