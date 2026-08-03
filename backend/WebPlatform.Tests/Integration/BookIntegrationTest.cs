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

        var queryResult = getResponse.Content.ReadFromJsonAsync<PagedResult<BookResponse>>().Result;

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

    [Theory]
    [InlineData("/api/books?publicationYear=999")] // below the allowed range
    [InlineData("/api/books?publicationYear=2101")] // above the allowed range
    public async Task GetBooks_WithPublicationYearOutOfRange_ShouldReturnBadRequest(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBooks_WithInvalidSortByValue_ShouldReturnBadRequest()
    {
        // Act: "Foo" is not a member of the BookSortField enum.
        var response = await _client.GetAsync("/api/books?sortBy=Foo");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBooks_ThenGetBooksSortedByPriceDescending_ShouldReturnBooksInDescendingOrder()
    {
        // Arrange: two books that are easy to tell apart by price.
        var cheaperBook = new Book
        {
            ISBN = "9780000000101",
            Title = "Sort Test Cheaper Book",
            Author = "Sort Test Author",
            Price = 10
        };

        var pricierBook = new Book
        {
            ISBN = "9780000000102",
            Title = "Sort Test Pricier Book",
            Author = "Sort Test Author",
            Price = 50
        };

        var postCheaperResponse = await _client.PostAsJsonAsync("/api/books", cheaperBook);
        postCheaperResponse.EnsureSuccessStatusCode();

        var postPricierResponse = await _client.PostAsJsonAsync("/api/books", pricierBook);
        postPricierResponse.EnsureSuccessStatusCode();

        // Act
        var getResponse = await _client.GetAsync(
            "/api/books?search=Sort Test&sortBy=Price&sortOrder=Descending");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var queryResult = await getResponse.Content.ReadFromJsonAsync<PagedResult<BookResponse>>();

        Assert.NotNull(queryResult);
        Assert.Equal(2, queryResult.Items.Count());
        Assert.Equal("Sort Test Pricier Book", queryResult.Items.First().Title);
        Assert.Equal("Sort Test Cheaper Book", queryResult.Items.Last().Title);

        // Cleanup.
        foreach (var book in queryResult.Items)
        {
            (await _client.DeleteAsync($"/api/books/{book.Id}")).EnsureSuccessStatusCode();
        }
    }

    [Fact]
    public async Task CreateBook_WithoutRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange: Title and Author are required but left at their default empty string.
        var invalidBook = new BookRequest
        {
            Price = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithNegativePrice_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidBook = new BookRequest
        {
            Title = "Some Book",
            Author = "Some Author",
            Price = -5
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(999)] // below the allowed range
    [InlineData(2101)] // above the allowed range
    public async Task CreateBook_WithPublicationYearOutOfRange_ShouldReturnBadRequest(int year)
    {
        // Arrange
        var invalidBook = new BookRequest
        {
            Title = "Some Book",
            Author = "Some Author",
            PublicationYear = year,
            Price = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_ThenGetBooksWithFilters_ShouldReturnOnlyMatchingBook()
    {
        // Arrange: two books that differ in language, condition and price,
        // so only one of them can satisfy all three filters at once.
        var matchingBook = new Book
        {
            ISBN = "9780132350884",
            Title = "Clean Code Filter Test",
            Author = "Robert C. Martin",
            Language = "English",
            Condition = BookCondition.New,
            Price = 20
        };

        var nonMatchingBook = new Book
        {
            ISBN = "9780134685991",
            Title = "Effective Java Filter Test",
            Author = "Joshua Bloch",
            Language = "German",
            Condition = BookCondition.Poor,
            Price = 200
        };

        var postMatchingResponse = await _client.PostAsJsonAsync("/api/books", matchingBook);
        postMatchingResponse.EnsureSuccessStatusCode();

        var postNonMatchingResponse = await _client.PostAsJsonAsync("/api/books", nonMatchingBook);
        postNonMatchingResponse.EnsureSuccessStatusCode();

        // Act: filter by language (lowercase, to also exercise the
        // case-insensitive comparison), condition and max price together.
        var getResponse = await _client.GetAsync(
            "/api/books?language=english&condition=New&maxPrice=50");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var queryResult = await getResponse.Content.ReadFromJsonAsync<PagedResult<BookResponse>>();

        Assert.NotNull(queryResult);
        Assert.Contains(queryResult.Items, b => b.ISBN == "9780132350884");
        Assert.DoesNotContain(queryResult.Items, b => b.ISBN == "9780134685991");

        // Cleanup.
        var matchingId = queryResult.Items.First(b => b.ISBN == "9780132350884").Id;
        (await _client.DeleteAsync($"/api/books/{matchingId}")).EnsureSuccessStatusCode();

        var allBooksResponse = await _client.GetAsync("/api/books?search=Effective Java Filter Test");
        var allBooksResult = await allBooksResponse.Content.ReadFromJsonAsync<PagedResult<BookResponse>>();
        var nonMatchingId = allBooksResult!.Items.First(b => b.ISBN == "9780134685991").Id;
        (await _client.DeleteAsync($"/api/books/{nonMatchingId}")).EnsureSuccessStatusCode();
    }
}