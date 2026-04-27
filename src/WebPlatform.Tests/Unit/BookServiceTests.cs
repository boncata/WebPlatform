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
}