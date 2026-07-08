using System.ComponentModel.DataAnnotations;

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
}
