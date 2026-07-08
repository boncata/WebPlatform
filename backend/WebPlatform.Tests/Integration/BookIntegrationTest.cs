using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WebPlatform.Api.Dtos;
using WebPlatform.Api.Models;

namespace WebPlatform.Tests.Integration;

public class BooksIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <summary>
    /// Integration test for the backend functionality of storing and
    /// retreiving books. Runs the Api "in memory" via WebApplicationFactory.
    /// The test currently uses the actual project database. Each time it is run,
    /// a new database entry is added. This is not ideal, but it is ok for the
    /// MVP. We will update this, after the MVP completion.
    /// </summary>
    /// <param name="factory">Factory object to run the Api.</param>
    public BooksIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        // Create the client, e.g. a web browser.
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/books?page=2&pageSize=5", 2, 5)] // test the pagination functionality
    [InlineData("/api/books?search=Design", 1, 20)] // test the search functionality and default pagination values
    public async Task CreateBook_ThenGetBooksWithPagination_BookShouldPersist(string url, int page_number, int page_size)
    {
        // Arrange
        var newBook = new Book
        {
            ISBN = "9780321125217",
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            PublicationYear = 2003,
            Publisher = "Addison-Wesley",
            Language = "English",
            Description = "Domain-driven design principles",
            Price = 55,
            Condition = BookCondition.New
        };

        // Act — POST
        var postResponse = await _client.PostAsJsonAsync(
            "/api/books",
            newBook);

        // Checks that the returned status code is 2xx. If not, something
        // went wrong and the test fails.
        postResponse.EnsureSuccessStatusCode();

        // Act — GET
        var getResponse = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var queryResult = getResponse.Content.ReadFromJsonAsync<PagedResult<Book>>().Result;

        Assert.NotNull(queryResult);
        Assert.Equal(page_number, queryResult.Page);
        Assert.Equal(page_size, queryResult.PageSize);
        Assert.Contains(
            queryResult.Items,
            b => b.ISBN == "9780321125217" &&
                b.Author == "Eric Evans" &&
                b.Title == "Domain-Driven Design" &&
                b.PublicationYear == 2003 &&
                b.Publisher == "Addison-Wesley" &&
                b.Language == "English" &&
                b.Description == "Domain-driven design principles" &&
                b.Price == 55 &&
                b.Condition == BookCondition.New
                    );

        // Cleanup: Delete the book we just created, to keep the database clean.
        // This is not ideal, but it is ok for the MVP. We will update this,
        // after the MVP completion.
        var deleteResponse = await _client.DeleteAsync(
            $"/api/books/{queryResult.Items.First(b => b.ISBN == "9780321125217").Id}");
        // Check that the delete was successful.
        deleteResponse.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("/api/books?page=-1&pageSize=5")]
    [InlineData("/api/books?page=1&pageSize=1000")]
    public async Task GetBooks_WithInvalidPaginationParameters_ShouldReturnBadRequest(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}