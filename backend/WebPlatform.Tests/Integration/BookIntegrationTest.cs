using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
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

    [Fact]
    public async Task CreateBook_ThenGetBooks_BookShouldPersist()
    {
        // Arrange
        var newBook = new Book
        {
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Price = 55
        };

        // Act — POST
        var postResponse = await _client.PostAsJsonAsync(
            "/api/books",
            newBook);

        // Checks that the returned status code is 2xx. If not, something
        // went wrong and the test fails.
        postResponse.EnsureSuccessStatusCode();

        // Act — GET
        var books = await _client.GetFromJsonAsync<List<Book>>(
            "/api/books");

        // Assert
        Assert.NotNull(books);
        Assert.Contains(
            books,
            b => b.Title == "Domain-Driven Design");
    }
}