using System.ComponentModel.DataAnnotations;
using WebPlatform.Api.Models;

namespace WebPlatform.Api.Dtos;

/**
* DTO (Data Transfer Object) class for the parameters of the book query.
* Putting this in a separate class allows us to easily add more parameters in the future,
* without having to change the method signature of the GetBooksAsync method.
*/
public class BookQueryParameters
{
    // The [Range] attribute is used to validate the input parameters.
    // If the input parameters are not within the specified range,a validation error will be returned.
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    // Search text field. Parameter is optional, so it can be null. If it is not null,
    // we will filter the books by the search text.
    public string? Search { get; set; }

    // Filtering parameters. All are optional; when null, the corresponding
    // filter is skipped entirely and does not narrow down the results.
    public string? Language { get; set; }

    public BookCondition? Condition { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxPrice { get; set; }

    public string? ISBN { get; set; }

    public string? Publisher { get; set; }

    // Same range and rationale as BookRequest.PublicationYear: 1000
    // comfortably predates the printing press, 2100 is a soft
    // "not absurdly far in the future" ceiling.
    [Range(1000, 2100)]
    public int? PublicationYear { get; set; }

    // Sorting parameters. When SortBy is null, results fall back to the
    // default Id order. SortOrder defaults to Ascending rather than being
    // nullable, since there's no meaningful difference between "not
    // specified" and "ascending" for a direction.
    public BookSortField? SortBy { get; set; }

    public SortDirection SortOrder { get; set; } = SortDirection.Ascending;
}
