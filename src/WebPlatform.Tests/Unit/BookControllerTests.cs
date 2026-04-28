using Microsoft.AspNetCore.Mvc;
using Moq;
using WebPlatform.Api.Controllers;
using WebPlatform.Api.Models;
using WebPlatform.Api.Services;

namespace WebPlatform.Tests.Unit.Controllers;

public class BooksControllerTests
{
    [Fact]
    public void GetBook_ShouldReturnOk_WhenBookExists()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        var book = new Book
        {
            Id = 1,
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Price = 30
        };

        mockService
            .Setup(service => service.GetBook(1))
            .Returns(book);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = controller.GetBook(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBook = Assert.IsType<Book>(okResult.Value);

        Assert.Equal(1, returnedBook.Id);
        Assert.Equal("Clean Code", returnedBook.Title);
    }

    [Fact]
    public void GetBook_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        mockService
            .Setup(service => service.GetBook(999))
            .Returns((Book?)null);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = controller.GetBook(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void AddBook_ShouldReturnOk_WhenBookIsCreated()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        var inputBook = new Book
        {
            Title = "Clean Architecture",
            Author = "Robert C. Martin",
            Price = 40
        };

        var createdBook = new Book
        {
            Id = 1,
            Title = "Clean Architecture",
            Author = "Robert C. Martin",
            Price = 40
        };

        mockService
            .Setup(service => service.AddBook(inputBook))
            .Returns(createdBook);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = controller.CreateBook(inputBook);

        // Assert
        var okResult = Assert.IsType<CreatedResult>(result);
        var returnedBook = Assert.IsType<Book>(okResult.Value);

        Assert.Equal(1, returnedBook.Id);
        Assert.Equal("Clean Architecture", returnedBook.Title);
    }
}