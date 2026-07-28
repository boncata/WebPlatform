using WebPlatform.Api.Dtos;

namespace WebPlatform.Api.Services;

// Interface for our books service. Must be implemented by
// any and all book services that we have.
public interface IBookService
{
    Task<PagedResult<BookResponse>> GetBooksAsync(BookQueryParameters queryParams);

    Task<BookResponse?> GetBookAsync(int id);

    Task<BookResponse> AddBookAsync(BookRequest request);

    Task<BookResponse?> UpdateBookAsync(int id, BookRequest bookRequest);

    Task<bool> DeleteBookAsync(int id);
}
