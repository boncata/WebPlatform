using WebPlatform.Api.Models;

namespace WebPlatform.Api.Services;

// Interface for our books service. Must be implemented by
// any and all book services that we have.
public interface IBookService
{
    IEnumerable<Book> GetBooks();

    Book? GetBook(int id);

    Book AddBook(Book book);

    Book? UpdateBook(Book book);

    bool DeleteBook(int id);
}
