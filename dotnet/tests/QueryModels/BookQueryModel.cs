using SieveQueryBuilder;

namespace tests.QueryModels;

/// <summary>
/// Query model for Book entity representing filterable and sortable properties
/// as configured in ApplicationSieveProcessor
/// </summary>
public class BookQueryModel : ISieveQueryModel
{
    /// <summary>
    /// Book ID - can filter and sort
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Book title - can filter and sort
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Number of pages - can filter and sort
    /// </summary>
    public int? Pages { get; set; }

    /// <summary>
    /// Creation date - can filter and sort
    /// </summary>
    public DateTime? Createdat { get; set; }
}
