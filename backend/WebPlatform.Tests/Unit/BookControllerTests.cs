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
            ISBN = "9780441172719",
            Title = "Dune",
            Author = "Frank Herbert",
            Description = "Sci-fi classic",
            Price = 19.99m,
            Condition = BookCondition.Good
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
        Assert.Equal("Dune", returnedBook.Title);
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
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedBook = Assert.IsType<Book>(okResult.Value);

        Assert.Equal(1, returnedBook.Id);
        Assert.Equal("Clean Architecture", returnedBook.Title);
    }

    [Fact]
    public void DeleteBook_ShouldReturnOk_WhenBookIsDeleted()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        mockService
            .Setup(service => service.DeleteBook(1))
            .Returns(true);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = controller.DeleteBook(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteBook_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBookService>();

        mockService
            .Setup(service => service.DeleteBook(999))
            .Returns(false);

        var controller = new BooksController(mockService.Object);

        // Act
        var result = controller.DeleteBook(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}