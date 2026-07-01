using WebPlatform.Api.Models;

namespace WebPlatform.Api.Dtos;

public class PagedResult<T>
{
    // Initialize Items to an empty collection to avoid null reference.
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
}