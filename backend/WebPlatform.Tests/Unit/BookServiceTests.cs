using Microsoft.EntityFrameworkCore;
using WebPlatform.Api.Data;
using WebPlatform.Api.Models;
using WebPlatform.Api.Services;

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
    public async Task AddBook_ShouldAddBookToDatabase()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BookService(context);

        var book = new Book
        {
            ISBN = "9780441172719",
            Title = "Dune",
            Author = "Frank Herbert",
            Description = "Sci-fi classic",
            Price = 19.99m,
            Condition = BookCondition.Good
        };

        // Act
        var result = await service.AddBookAsync(book);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Dune", result.Title);
        Assert.Equal("Frank Herbert", result.Author);
        Assert.Equal("Sci-fi classic", result.Description);
        Assert.Equal(19.99m, result.Price);
        Assert.Equal(BookCondition.Good, result.Condition);
        Assert.Equal(1, context.Books.Count());
    }

    [Fact]
    public async Task GetBook_ShouldReturnCorrectBook()
    {
        // Arrange
        var context = CreateDbContext();

        var book = new Book
        {
            ISBN = "9780441172719",
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt",
            Description = "Practical programming advice",
            Price = 45,
            Condition = BookCondition.Excellent
        };

        context.Books.Add(book);
        context.SaveChanges();

        var service = new BookService(context);

        // Act
        var result = await service.GetBookAsync(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal("The Pragmatic Programmer", result.Title);
        Assert.Equal("Andrew Hunt", result.Author);
        Assert.Equal("Practical programming advice", result.Description);
        Assert.Equal(45, result.Price);
        Assert.Equal(BookCondition.Excellent, result.Condition);
    }

    [Fact]
    public async Task GetBook_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BookService(context);

        int nonExistingId = 999;

        // Act
        var result = await service.GetBookAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnAllBooks()
    {
        // Arrange
        var context = CreateDbContext();

        var books = new List<Book>
        {
            new Book
            {
                ISBN = "9780441172719",
                Title = "Dune",
                Author = "Frank Herbert",
                Description = "Sci-fi classic",
                Price = 19.99m,
                Condition = BookCondition.New
            },
            new Book
            {
                ISBN = "9780441172719",
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt",
                Description = "Practical programming advice",
                Price = 10,
                Condition = BookCondition.Poor
            }
        };

        context.Books.AddRange(books);
        context.SaveChanges();

        var service = new BookService(context);

        // Act
        var result = await service.GetBooksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        Assert.Contains(result, b => b.Title == "Dune");
        Assert.Contains(result, b => b.Title == "The Pragmatic Programmer");
    }

    [Fact]
    public async Task DeleteBook_ShouldRemoveBookFromDatabase()
    {
        // Arrange
        var context = CreateDbContext();

        var book = new Book
        {
            ISBN = "9780441172719",
            Title = "Dune",
            Author = "Frank Herbert",
            Description = "Sci-fi classic",
            Price = 19.99m,
            Condition = BookCondition.Good
        };

        context.Books.Add(book);
        context.SaveChanges();

        // Quick check to ensure the book was added before we delete it.
        Assert.Contains(context.Books, b => b.Title == "Dune");

        var service = new BookService(context);

        // Act
        var result = await service.DeleteBookAsync(book.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(context.Books);
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnFalse_WhenBookDoesNotExist()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BookService(context);

        int nonExistingId = 999;

        // Act
        var result = await service.DeleteBookAsync(nonExistingId);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(BookCondition.New)]
    [InlineData(BookCondition.LikeNew)]
    [InlineData(BookCondition.Excellent)]
    [InlineData(BookCondition.Good)]
    [InlineData(BookCondition.Fair)]
    [InlineData(BookCondition.Poor)]
    public async Task BookCondition_ShouldPersistCorrectly(
        BookCondition condition)
    {
        var context = CreateDbContext();

        var book = new Book
        {
            Title = "Test",
            Author = "Test",
            Condition = condition
        };

        context.Books.Add(book);
        await context.SaveChangesAsync();

        var saved = await context.Books.FirstAsync();

        Assert.Equal(condition, saved.Condition);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnUpdatedBook_WhenBookIsUpdated()
    {
        // Arrange
        var context = CreateDbContext();

        var book = new Book
        {
            Id = 1,
            ISBN = "9780441172719",
            Title = "Dune",
            Author = "Frank Herbert",
            Description = "Sci-fi classic",
            Price = 19.99m,
            Condition = BookCondition.Good
        };

        context.Books.Add(book);
        context.SaveChanges();

        var updatedBook = new Book
        {
            Id = 1,
            ISBN = "9780441172719",
            Title = "Dune - Updated",
            Author = "Frank Herbert",
            Description = "Sci-fi classic - Updated",
            Price = 29.99m,
            Condition = BookCondition.Excellent
        };

        var service = new BookService(context);

        // Act
        var result = await service.UpdateBookAsync(updatedBook);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedBook.Id, result.Id);
        Assert.Equal("Dune - Updated", updatedBook.Title);
        Assert.Equal("Sci-fi classic - Updated", updatedBook.Description);
        Assert.Equal(book.ISBN, result.ISBN);
        Assert.Equal(BookCondition.Excellent, result.Condition);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BookService(context);

        // Do not add nonExistingBook to the context, so it
        // simulates a book that does not exist in the database.
        var nonExistingBook = new Book
        {
            Id = 999,
            ISBN = "9780441172719",
            Title = "Non-existing Book",
            Author = "Unknown",
            Description = "This book does not exist in the database",
            Price = 0,
            Condition = BookCondition.Poor
        };

        // Act
        var result = await service.UpdateBookAsync(nonExistingBook);

        // Assert
        Assert.Null(result);
    }
}
