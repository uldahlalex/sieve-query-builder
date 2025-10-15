using SieveQueryBuilder;
using tests.QueryModels;

namespace tests;

/// <summary>
/// Tests for type-safe query models that enforce Sieve processor configuration
/// </summary>
public class SieveQueryBuilderWithCustomQueryModelsTests
{
    private readonly ITestOutputHelper _outputHelper;

    public SieveQueryBuilderWithCustomQueryModelsTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void QueryModel_AllowsOnlyConfiguredProperties()
    {
        // Arrange & Act - Using AuthorQueryModel instead of Author entity
        var query = SieveQueryBuilder<AuthorQueryModel>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterGreaterThanOrEqual(a => a.Createdat, DateTime.Now.AddDays(-7))
            .SortByDescending(a => a.Createdat)
            .BuildQueryString();

        // Assert
        Assert.Contains("Name%40%3DBob", query);  // URL encoded Name@=Bob
        Assert.Contains("Createdat%3E%3D", query);  // URL encoded Createdat>=
        Assert.Contains("-Createdat", query);

        _outputHelper.WriteLine($"Query with AuthorQueryModel: {query}");
    }

    [Fact]
    public void QueryModel_SupportsCustomMappedProperties_WithTypeSafety()
    {
        // Arrange & Act - BooksCount is now a typed property!
        var query = SieveQueryBuilder<AuthorQueryModel>.Create()
            .FilterGreaterThanOrEqual(a => a.BooksCount, 5)  // Type-safe lambda!
            .SortByDescending(a => a.BooksCount)              // Type-safe lambda!
            .BuildSieveModel();

        // Assert
        Assert.Equal("BooksCount>=5", query.Filters);
        Assert.Equal("-BooksCount", query.Sorts);

        _outputHelper.WriteLine($"Type-safe custom property filter: {query.Filters}");
        _outputHelper.WriteLine($"Type-safe custom property sort: {query.Sorts}");
    }

    [Fact]
    public void QueryModel_ComplexQueryWithCustomProperties()
    {
        // Arrange & Act - Real-world scenario with custom mapped property
        var sieveModel = SieveQueryBuilder<AuthorQueryModel>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterGreaterThanOrEqual(a => a.Createdat, DateTime.Now.AddDays(-30))
            .FilterGreaterThanOrEqual(a => a.BooksCount, 3)  // Custom property with IntelliSense!
            .SortByDescending(a => a.Createdat)
            .SortBy(a => a.Name)
            .Page(1)
            .PageSize(20)
            .BuildSieveModel();

        // Assert
        Assert.Contains("Name@=Bob", sieveModel.Filters);
        Assert.Contains("Createdat>=", sieveModel.Filters);
        Assert.Contains("BooksCount>=3", sieveModel.Filters);
        Assert.Equal("-Createdat,Name", sieveModel.Sorts);

        _outputHelper.WriteLine($"Complex query with custom properties:");
        _outputHelper.WriteLine($"  Filters: {sieveModel.Filters}");
        _outputHelper.WriteLine($"  Sorts: {sieveModel.Sorts}");
    }

    [Fact]
    public void QueryModel_ParseQueryString_WorksWithQueryModel()
    {
        // Arrange
        var queryString = "filters=Name@=Bob,BooksCount>=5&sorts=-BooksCount,Name&page=2";

        // Act
        var builder = SieveQueryBuilder<AuthorQueryModel>.ParseQueryString(queryString);
        var filters = builder.GetFilters();
        var sorts = builder.GetSorts();

        // Assert
        Assert.Equal(2, filters.Count);
        Assert.Equal("Name", filters[0].PropertyName);
        Assert.Equal("BooksCount", filters[1].PropertyName);
        Assert.Equal(2, sorts.Count);
        Assert.Equal("BooksCount", sorts[0].PropertyName);
        Assert.True(sorts[0].IsDescending);

        _outputHelper.WriteLine($"Parsed {filters.Count} filters from query model");
        _outputHelper.WriteLine($"Parsed {sorts.Count} sorts from query model");
    }

    [Fact]
    public void QueryModel_RoundTrip_ParseModifyRebuild()
    {
        // Arrange - Start with query string
        var originalQuery = "filters=Name@=Bob&sorts=Name&page=1";

        // Act - Parse into query model, modify with type-safe methods, rebuild
        var builder = SieveQueryBuilder<AuthorQueryModel>.ParseQueryString(originalQuery);
        builder.FilterGreaterThanOrEqual(a => a.BooksCount, 5)  // Type-safe addition!
               .SortByDescending(a => a.Createdat);

        var newQuery = builder.BuildQueryString();
        var filters = builder.GetFilters();

        // Assert
        Assert.Equal(2, filters.Count);
        Assert.Contains("BooksCount%3E%3D5", newQuery);  // URL encoded BooksCount>=5
        Assert.Contains("-Createdat", newQuery);

        _outputHelper.WriteLine($"Original: {originalQuery}");
        _outputHelper.WriteLine($"Modified: {newQuery}");
    }

    [Fact]
    public void QueryModel_BookQueryModel_WorksCorrectly()
    {
        // Arrange & Act - Test with different entity query model
        var query = SieveQueryBuilder<BookQueryModel>.Create()
            .FilterGreaterThan(b => b.Pages, 200)
            .FilterLessThan(b => b.Pages, 500)
            .FilterContains(b => b.Title, "Test")
            .SortByDescending(b => b.Createdat)
            .BuildSieveModel();

        // Assert
        Assert.Contains("Pages>200", query.Filters);
        Assert.Contains("Pages<500", query.Filters);
        Assert.Contains("Title@=Test", query.Filters);
        Assert.Equal("-Createdat", query.Sorts);

        _outputHelper.WriteLine($"BookQueryModel filters: {query.Filters}");
    }

    [Fact]
    public void QueryModel_AllOperators_WorkWithTypeSafety()
    {
        // Arrange & Act - Test all operators with query model
        var builder = SieveQueryBuilder<AuthorQueryModel>.Create();

        var equals = builder.FilterEquals(a => a.Name, "Bob").BuildFiltersString();

        builder = SieveQueryBuilder<AuthorQueryModel>.Create();
        var notEquals = builder.FilterNotEquals(a => a.Name, "Alice").BuildFiltersString();

        builder = SieveQueryBuilder<AuthorQueryModel>.Create();
        var contains = builder.FilterContains(a => a.Name, "test").BuildFiltersString();

        builder = SieveQueryBuilder<AuthorQueryModel>.Create();
        var startsWith = builder.FilterStartsWith(a => a.Name, "B").BuildFiltersString();

        builder = SieveQueryBuilder<AuthorQueryModel>.Create();
        var greaterThan = builder.FilterGreaterThan(a => a.BooksCount, 5).BuildFiltersString();

        builder = SieveQueryBuilder<AuthorQueryModel>.Create();
        var lessThan = builder.FilterLessThan(a => a.BooksCount, 10).BuildFiltersString();

        // Assert
        Assert.Equal("Name==Bob", equals);
        Assert.Equal("Name!=Alice", notEquals);
        Assert.Equal("Name@=test", contains);
        Assert.Equal("Name_=B", startsWith);
        Assert.Equal("BooksCount>5", greaterThan);
        Assert.Equal("BooksCount<10", lessThan);

        _outputHelper.WriteLine("All operators work with type-safe query models");
    }

    [Fact]
    public void QueryModel_HasFilter_WorksWithQueryModelProperties()
    {
        // Arrange
        var builder = SieveQueryBuilder<AuthorQueryModel>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterGreaterThanOrEqual(a => a.BooksCount, 3);

        // Act & Assert
        Assert.True(builder.HasFilter("Name"));
        Assert.True(builder.HasFilter("BooksCount"));
        Assert.False(builder.HasFilter("Id"));

        _outputHelper.WriteLine($"HasFilter works for query model properties");
    }

    [Fact]
    public void QueryModel_FromSieveModel_PreservesTypeUse()
    {
        // Arrange
        var model = new Sieve.Models.SieveModel
        {
            Filters = "Name@=Bob,BooksCount>=5",
            Sorts = "-BooksCount,Name",
            Page = 2,
            PageSize = 20
        };

        // Act - Create from SieveModel and add more type-safe filters
        var builder = SieveQueryBuilder<AuthorQueryModel>.FromSieveModel(model);
        builder.FilterGreaterThanOrEqual(a => a.Createdat, DateTime.Now.AddDays(-30));

        var rebuilt = builder.BuildSieveModel();
        var filters = builder.GetFilters();

        // Assert
        Assert.Equal(3, filters.Count);  // 2 from model + 1 added
        Assert.Contains("Name@=Bob", rebuilt.Filters);
        Assert.Contains("BooksCount>=5", rebuilt.Filters);
        Assert.Contains("Createdat>=", rebuilt.Filters);

        _outputHelper.WriteLine($"FromSieveModel + type-safe additions: {rebuilt.Filters}");
    }
}
