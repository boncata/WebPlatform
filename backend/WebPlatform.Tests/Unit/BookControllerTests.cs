using Microsoft.AspNetCore.Mvc;
using Moq;
using WebPlatform.Api.Controllers;
using WebPlatform.Api.Dtos;
using WebPlatform.Api.Models;
using WebPlatform.Api.Services;

namespace WebPlatform.Tests.Unit.Controllers;

public class BooksControllerTests
{
    [Fact]
    public async Task GetBooks_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        var pagedResult = new PagedResult<BookResponse>
        {
            Items = new List<BookResponse>
            {
                new BookResponse { Id = 1, Title = "Dune", Author = "Frank Herbert" },
                new BookResponse { Id = 2, Title = "The Pragmatic Programmer of Dune", Author = "Andrew Hunt and David Thomas" }
            },
            Page = 1,
            PageSize = 2,
            TotalCount = 2
        };

        var queryParams = new BookQueryParameters
        {
            Page = 1,
            PageSize = 2,
            Search = "Dune"
        };

        mockService
            .Setup(service => service.GetBooksAsync(queryParams))
            .ReturnsAsync(pagedResult);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.GetBooks(queryParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedQueryResult = Assert.IsType<PagedResult<BookResponse>>(okResult.Value);

        Assert.Equal(2, returnedQueryResult.TotalCount);
        Assert.Equal(1, returnedQueryResult.Page);
        Assert.Equal(2, returnedQueryResult.PageSize);
        Assert.Equal(1, returnedQueryResult.Items.First().Id);
        Assert.Equal("Dune", returnedQueryResult.Items.First().Title);
    }


    [Fact]
    public async Task GetBook_ShouldReturnOk_WhenBookExists()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        var bookResponse = new BookResponse
        {
            Id = 1,
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

        mockService
            .Setup(service => service.GetBookAsync(1))
            .ReturnsAsync(bookResponse);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.GetBook(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBook = Assert.IsType<BookResponse>(okResult.Value);

        Assert.Equal(1, returnedBook.Id);
        Assert.Equal("Dune", returnedBook.Title);
    }

    [Fact]
    public async Task GetBook_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        mockService
            .Setup(service => service.GetBookAsync(999))
            .ReturnsAsync((BookResponse?)null);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.GetBook(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddBook_ShouldReturnOk_WhenBookIsCreated()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        var inputBookRequest = new BookRequest
        {
            Title = "Clean Architecture",
            Author = "Robert C. Martin",
            Price = 40
        };

        var inputBookResponse = new BookResponse
        {
            Title = "Clean Architecture",
            Author = "Robert C. Martin",
            Price = 40
        };

        mockService
            .Setup(service => service.AddBookAsync(inputBookRequest))
            .ReturnsAsync(inputBookResponse);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.CreateBook(inputBookRequest);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedBook = Assert.IsType<BookResponse>(okResult.Value);

        Assert.Equal("Clean Architecture", returnedBook.Title);
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnOk_WhenBookIsDeleted()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        mockService
            .Setup(service => service.DeleteBookAsync(1))
            .ReturnsAsync(true);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.DeleteBook(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        mockService
            .Setup(service => service.DeleteBookAsync(999))
            .ReturnsAsync(false);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.DeleteBook(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnOk_WhenBookIsUpdated()
    {
        // Arrange
        var mockService = new Mock<IBookService>();
        int id = 1;

        var inputBookRequest = new BookRequest
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Price = 35
        };

        var bookReturn = new BookResponse
        {
            Id = id,
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Price = 35
        };

        mockService
            .Setup(service => service.UpdateBookAsync(id, inputBookRequest))
            .ReturnsAsync(bookReturn);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.UpdateBook(id, inputBookRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBook = Assert.IsType<BookResponse>(okResult.Value);

        Assert.Equal(id, returnedBook.Id);
        Assert.Equal("Clean Code", returnedBook.Title);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        int non_existent_id = 999;

        var inputBookRequest = new BookRequest
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Price = 35
        };

        mockService
            .Setup(service => service
            .UpdateBookAsync(non_existent_id, inputBookRequest))
            .ReturnsAsync((BookResponse?)null);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = await controller.UpdateBook(non_existent_id, inputBookRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}