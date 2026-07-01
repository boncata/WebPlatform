using Microsoft.EntityFrameworkCore;
using WebPlatform.Api.Data;
using WebPlatform.Api.Dtos;
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

        var bookRequest = new BookRequest
        {
            ISBN = "9780441172719",
            Title = "Dune",
            Author = "Frank Herbert",
            PublicationYear = 1965,
            Publisher = "Chilton Books",
            Language = "English",
            Description = "Sci-fi classic",
            Price = 19.99m,
            Condition = BookCondition.Good
        };

        // Act
        var result = await service.AddBookAsync(bookRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("9780441172719", result.ISBN);
        Assert.Equal("Dune", result.Title);
        Assert.Equal("Frank Herbert", result.Author);
        Assert.Equal(1965, result.PublicationYear);
        Assert.Equal("Chilton Books", result.Publisher);
        Assert.Equal("English", result.Language);
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
            PublicationYear = 1999,
            Publisher = "Addison-Wesley",
            Language = "English",
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
        Assert.Equal("9780441172719", result.ISBN);
        Assert.Equal("The Pragmatic Programmer", result.Title);
        Assert.Equal("Andrew Hunt", result.Author);
        Assert.Equal(1999, result.PublicationYear);
        Assert.Equal("Addison-Wesley", result.Publisher);
        Assert.Equal("English", result.Language);
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

    [Theory]
    [InlineData(30, 3, 1)]
    [InlineData(30, 5, 2)]
    public async Task GetBooks_ReturnsCorrectData_WhenCalledWithSpecificPageNumberAndSize(
        int num_books, int page_size, int page_number)
    {
        // Arrange
        var context = CreateDbContext();

        var books = new List<Book>();
        for (int i = 1; i <= num_books; i++)
        {
            books.Add(new Book
            {
                ISBN = $"97804411727{i:D2}",
                Title = $"Book {i}",
                Author = $"Author {i}",
                PublicationYear = 2000 + i,
                Publisher = $"Publisher {i}",
                Language = "English",
                Description = $"Description for Book {i}",
                Price = 10 + i,
                Condition = BookCondition.New
            });
        }

        context.Books.AddRange(books);
        context.SaveChanges();

        var service = new BookService(context);

        var queryParams = new BookQueryParameters
        {
            Page = page_number,
            PageSize = page_size
        };

        // Act
        var result = await service.GetBooksAsync(queryParams);

        // Assert
        Assert.Equal(num_books, result.TotalCount);
        Assert.Equal(page_number, result.Page);
        Assert.Equal(page_size, result.PageSize);
        Assert.Equal((page_number - 1) * page_size + 1, result.Items.First().Id);
        Assert.Contains(result.Items, b => b.Title == $"Book {(page_number - 1) * page_size + 1}");
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
            PublicationYear = 1965,
            Publisher = "Chilton Books",
            Language = "English",
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

        int id = 1;

        var book = new Book
        {
            Id = id,
            ISBN = "9780441172719",
            Title = "Dune",
            Author = "Frank Herbert",
            PublicationYear = 1965,
            Publisher = "Chilton Books",
            Language = "English",
            Description = "Sci-fi classic",
            Price = 19.99m,
            Condition = BookCondition.Good
        };

        context.Books.Add(book);
        context.SaveChanges();

        var updatedBookRequest = new BookRequest
        {
            ISBN = "9780441172719",
            Title = "Dune - Updated",
            Author = "Frank Herbert",
            PublicationYear = 1965,
            Publisher = "Chilton Books - Updated",
            Language = "Pirate-English",
            Description = "Sci-fi classic - Updated",
            Price = 29.99m,
            Condition = BookCondition.Excellent
        };

        var service = new BookService(context);

        // Act
        var result = await service.UpdateBookAsync(id, updatedBookRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Dune - Updated", updatedBookRequest.Title);
        Assert.Equal("Sci-fi classic - Updated", updatedBookRequest.Description);
        Assert.Equal(book.ISBN, result.ISBN);
        Assert.Equal("Chilton Books - Updated", result.Publisher);
        Assert.Equal("Pirate-English", result.Language);
        Assert.Equal(29.99m, result.Price);
        Assert.Equal(BookCondition.Excellent, result.Condition);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BookService(context);

        int nonExistingId = 999;

        // Do not add nonExistingBook to the context, so it
        // simulates a book that does not exist in the database.
        var nonExistentBookRequest = new BookRequest
        {
            ISBN = "9780441172719",
            Title = "Non-existing Book",
            Author = "Unknown",
            Description = "This book does not exist in the database",
            Price = 0,
            Condition = BookCondition.Poor
        };

        // Act
        var result = await service.UpdateBookAsync(nonExistingId, nonExistentBookRequest);

        // Assert
        Assert.Null(result);
    }
}
