using SieveQueryBuilder;

namespace tests.QueryModels;

/// <summary>
/// Query model for Author entity representing filterable and sortable properties
/// as configured in ApplicationSieveProcessor
/// </summary>
public class AuthorQueryModel : ISieveQueryModel
{
    /// <summary>
    /// Author ID - can filter and sort
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Author name - can filter and sort
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Creation date - can filter and sort
    /// </summary>
    public DateTime? Createdat { get; set; }

    /// <summary>
    /// Number of books (custom mapped property from Books.Count)
    /// Configured as: mapper.Property&lt;Author&gt;(a => a.Books.Count).HasName("BooksCount")
    /// </summary>
    public int? BooksCount { get; set; }
}
