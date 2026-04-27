using Microsoft.EntityFrameworkCore;
using WebPlatform.Api.Data;
using WebPlatform.Api.Models;
using WebPlatform.Api.Services;
using Xunit;

namespace WebPlatform.Tests.Unit.Services;

public class BookServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void AddBook_ShouldAddBookToDatabase()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BookService(context);

        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Price = 30
        };

        // Act
        var result = service.AddBook(book);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Clean Code", result.Title);
        Assert.Equal(1, context.Books.Count());
    }

    [Fact]
    public void GetBook_ShouldReturnCorrectBook()
    {
        // Arrange
        var context = CreateDbContext();

        var book = new Book
        {
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt",
            Price = 45
        };

        context.Books.Add(book);
        context.SaveChanges();

        var service = new BookService(context);

        // Act
        var result = service.GetBook(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal("The Pragmatic Programmer", result.Title);
        Assert.Equal("Andrew Hunt", result.Author);
        Assert.Equal(45, result.Price);
    }

    [Fact]
    public void GetBook_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BookService(context);

        int nonExistingId = 999;

        // Act
        var result = service.GetBook(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetBooks_ShouldReturnAllBooks()
    {
        // Arrange
        var context = CreateDbContext();

        var books = new List<Book>
        {
            new Book
            {
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Price = 30
            },
            new Book
            {
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt",
                Price = 45
            }
        };

        context.Books.AddRange(books);
        context.SaveChanges();

        var service = new BookService(context);

        // Act
        var result = service.GetBooks();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        Assert.Contains(result, b => b.Title == "Clean Code");
        Assert.Contains(result, b => b.Title == "The Pragmatic Programmer");
    }
}