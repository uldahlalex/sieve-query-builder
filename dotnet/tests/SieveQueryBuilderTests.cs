using SieveQueryBuilder;
using tests.Entities;

namespace tests;

/// <summary>
/// Demonstrates type-safe Sieve query building
/// </summary>
public class SieveQueryBuilderTests
{
    private readonly ITestOutputHelper _outputHelper;

    public SieveQueryBuilderTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void BuildFilterEquals_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterEquals(a => a.Name, "Bob_5")
            .PageSize(10)
            .BuildFiltersString();

        // Assert
        Assert.Equal("Name==Bob_5", query);
        _outputHelper.WriteLine($"Filter: {query}");
    }

    [Fact]
    public void BuildFilterContains_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "_5")
            .BuildFiltersString();

        // Assert
        Assert.Equal("Name@=_5", query);
        _outputHelper.WriteLine($"Filter: {query}");
    }

    [Fact]
    public void BuildMultipleFilters_CombinesCorrectly()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterNotEquals(a => a.Name, "Bob_0")
            .BuildFiltersString();

        // Assert
        Assert.Equal("Name@=Bob,Name!=Bob_0", query);
        _outputHelper.WriteLine($"Filter: {query}");
    }

    [Fact]
    public void BuildSortByAscending_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .SortBy(a => a.Name)
            .BuildSortsString();

        // Assert
        Assert.Equal("Name", query);
        _outputHelper.WriteLine($"Sort: {query}");
    }

    [Fact]
    public void BuildSortByDescending_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .SortByDescending(a => a.Name)
            .BuildSortsString();

        // Assert
        Assert.Equal("-Name", query);
        _outputHelper.WriteLine($"Sort: {query}");
    }

    [Fact]
    public void BuildMultipleSorts_CombinesCorrectly()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .SortByDescending(a => a.Createdat)
            .SortBy(a => a.Name)
            .BuildSortsString();

        // Assert
        Assert.Equal("-Createdat,Name", query);
        _outputHelper.WriteLine($"Sort: {query}");
    }

    [Fact]
    public void BuildSieveModel_CreatesCompleteModel()
    {
        // Arrange & Act
        var sieveModel = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .SortBy(a => a.Name)
            .Page(2)
            .PageSize(10)
            .BuildSieveModel();

        // Assert
        Assert.Equal("Name@=Bob", sieveModel.Filters);
        Assert.Equal("Name", sieveModel.Sorts);
        Assert.Equal(2, sieveModel.Page);
        Assert.Equal(10, sieveModel.PageSize);

        _outputHelper.WriteLine($"Filters: {sieveModel.Filters}");
        _outputHelper.WriteLine($"Sorts: {sieveModel.Sorts}");
        _outputHelper.WriteLine($"Page: {sieveModel.Page}");
        _outputHelper.WriteLine($"PageSize: {sieveModel.PageSize}");
    }

    [Fact]
    public void BuildQueryString_CreatesCompleteHttpQueryString()
    {
        // Arrange & Act
        var queryString = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterNotEquals(a => a.Name, "Bob_0")
            .SortBy(a => a.Name)
            .Page(2)
            .PageSize(10)
            .BuildQueryString();

        // Assert
        Assert.Contains("filters=Name%40%3DBob%2CName%21%3DBob_0", queryString);
        Assert.Contains("sorts=Name", queryString);
        Assert.Contains("page=2", queryString);
        Assert.Contains("pageSize=10", queryString);

        _outputHelper.WriteLine($"Query String: {queryString}");
    }

    [Fact]
    public void BuildWithCustomPropertyName_ForMappedProperties()
    {
        // Arrange & Act - BooksCount is mapped in ApplicationSieveProcessor
        var query = SieveQueryBuilder<Author>.Create()
            .FilterByName("BooksCount", ">=", 5)
            .SortByName("BooksCount", descending: true)
            .BuildFiltersString();

        var sort = SieveQueryBuilder<Author>.Create()
            .SortByName("BooksCount", descending: true)
            .BuildSortsString();

        // Assert
        Assert.Equal("BooksCount>=5", query);
        Assert.Equal("-BooksCount", sort);

        _outputHelper.WriteLine($"Filter: {query}");
        _outputHelper.WriteLine($"Sort: {sort}");
    }

    [Fact]
    public void BuildComparisonFilters_AllOperators()
    {
        // Arrange & Act
        var builder = SieveQueryBuilder<Book>.Create();

        var greaterThan = builder.FilterGreaterThan(b => b.Pages, 200).BuildFiltersString();

        builder = SieveQueryBuilder<Book>.Create();
        var lessThan = builder.FilterLessThan(b => b.Pages, 500).BuildFiltersString();

        builder = SieveQueryBuilder<Book>.Create();
        var greaterOrEqual = builder.FilterGreaterThanOrEqual(b => b.Pages, 200).BuildFiltersString();

        builder = SieveQueryBuilder<Book>.Create();
        var lessOrEqual = builder.FilterLessThanOrEqual(b => b.Pages, 500).BuildFiltersString();

        // Assert
        Assert.Equal("Pages>200", greaterThan);
        Assert.Equal("Pages<500", lessThan);
        Assert.Equal("Pages>=200", greaterOrEqual);
        Assert.Equal("Pages<=500", lessOrEqual);

        _outputHelper.WriteLine($"Greater than: {greaterThan}");
        _outputHelper.WriteLine($"Less than: {lessThan}");
        _outputHelper.WriteLine($"Greater or equal: {greaterOrEqual}");
        _outputHelper.WriteLine($"Less or equal: {lessOrEqual}");
    }

    [Fact]
    public void BuildDateTimeFilter_WorksCorrectly()
    {
        // Arrange
        var targetDate = new DateTime(2024, 1, 1);

        // Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThanOrEqual(a => a.Createdat, targetDate)
            .BuildFiltersString();

        // Assert
        Assert.Contains("Createdat>=", query);
        _outputHelper.WriteLine($"DateTime Filter: {query}");
    }

    [Fact]
    public void BuildDateTimeFilter_UtcKind_ProducesISO8601Format()
    {
        // Arrange
        var utcDate = new DateTime(2024, 6, 10, 16, 48, 5, 123, DateTimeKind.Utc);

        // Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThan(a => a.Createdat, utcDate)
            .BuildFiltersString();

        // Assert
        Assert.Equal("Createdat>2024-06-10T16:48:05.123Z", query);
        _outputHelper.WriteLine($"UTC DateTime Filter: {query}");
    }

    [Fact]
    public void BuildDateTimeFilter_UnspecifiedKind_ConvertsToUtcISO8601()
    {
        // Arrange
        var unspecifiedDate = new DateTime(2024, 6, 10, 16, 48, 5, 123, DateTimeKind.Unspecified);

        // Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThan(a => a.Createdat, unspecifiedDate)
            .BuildFiltersString();

        // Assert - Should convert to UTC and format as ISO 8601
        Assert.Contains("Createdat>", query);
        Assert.Contains("Z", query); // Should have UTC indicator
        Assert.Matches(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z", query.Substring(query.IndexOf('>') + 1));
        _outputHelper.WriteLine($"Unspecified DateTime Filter: {query}");
    }

    [Fact]
    public void BuildDateTimeFilter_LocalKind_ConvertsToUtcISO8601()
    {
        // Arrange
        var localDate = new DateTime(2024, 6, 10, 16, 48, 5, 123, DateTimeKind.Local);

        // Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThan(a => a.Createdat, localDate)
            .BuildFiltersString();

        // Assert - Should convert to UTC and format as ISO 8601
        Assert.Contains("Createdat>", query);
        Assert.Contains("Z", query); // Should have UTC indicator
        Assert.Matches(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z", query.Substring(query.IndexOf('>') + 1));
        _outputHelper.WriteLine($"Local DateTime Filter: {query}");
    }

    [Fact]
    public void BuildDateTimeFilter_RealWorldScenario_PostgresqlCompatible()
    {
        // Arrange - Simulate the exact scenario from the issue description
        var cutoffDate = DateTime.UtcNow.AddDays(-500);

        // Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThan(a => a.Createdat, cutoffDate)
            .BuildSieveModel();

        // Assert
        Assert.Contains("Createdat>", query.Filters);
        Assert.Contains("Z", query.Filters); // UTC indicator present
        // Verify it's in ISO 8601 format (culture-independent)
        Assert.Matches(@"Createdat>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z", query.Filters);

        _outputHelper.WriteLine($"PostgreSQL-compatible DateTime Filter: {query.Filters}");
        _outputHelper.WriteLine($"Format is culture-independent ISO 8601 with UTC indicator");
    }

    [Fact]
    public void BuildDateTimeFilter_AllComparisonOperators_UseISO8601()
    {
        // Arrange
        var testDate = new DateTime(2024, 1, 15, 10, 30, 45, 678, DateTimeKind.Utc);
        var expectedFormat = "2024-01-15T10:30:45.678Z";

        // Act
        var queryEquals = SieveQueryBuilder<Author>.Create()
            .FilterEquals(a => a.Createdat, testDate)
            .BuildFiltersString();

        var queryNotEquals = SieveQueryBuilder<Author>.Create()
            .FilterNotEquals(a => a.Createdat, testDate)
            .BuildFiltersString();

        var queryGreaterThan = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThan(a => a.Createdat, testDate)
            .BuildFiltersString();

        var queryLessThan = SieveQueryBuilder<Author>.Create()
            .FilterLessThan(a => a.Createdat, testDate)
            .BuildFiltersString();

        var queryGreaterOrEqual = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThanOrEqual(a => a.Createdat, testDate)
            .BuildFiltersString();

        var queryLessOrEqual = SieveQueryBuilder<Author>.Create()
            .FilterLessThanOrEqual(a => a.Createdat, testDate)
            .BuildFiltersString();

        // Assert
        Assert.Equal($"Createdat=={expectedFormat}", queryEquals);
        Assert.Equal($"Createdat!={expectedFormat}", queryNotEquals);
        Assert.Equal($"Createdat>{expectedFormat}", queryGreaterThan);
        Assert.Equal($"Createdat<{expectedFormat}", queryLessThan);
        Assert.Equal($"Createdat>={expectedFormat}", queryGreaterOrEqual);
        Assert.Equal($"Createdat<={expectedFormat}", queryLessOrEqual);

        _outputHelper.WriteLine("All comparison operators use ISO 8601 format:");
        _outputHelper.WriteLine($"  Equals: {queryEquals}");
        _outputHelper.WriteLine($"  NotEquals: {queryNotEquals}");
        _outputHelper.WriteLine($"  GreaterThan: {queryGreaterThan}");
        _outputHelper.WriteLine($"  LessThan: {queryLessThan}");
        _outputHelper.WriteLine($"  GreaterOrEqual: {queryGreaterOrEqual}");
        _outputHelper.WriteLine($"  LessOrEqual: {queryLessOrEqual}");
    }

    [Fact]
    public void BuildDateTimeFilter_FilterByName_UseISO8601()
    {
        // Arrange
        var testDate = new DateTime(2024, 1, 15, 10, 30, 45, 678, DateTimeKind.Utc);
        var expectedFormat = "2024-01-15T10:30:45.678Z";

        // Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterByName("CustomDateField", ">=", testDate)
            .BuildFiltersString();

        // Assert
        Assert.Equal($"CustomDateField>={expectedFormat}", query);
        _outputHelper.WriteLine($"FilterByName with DateTime: {query}");
    }

    [Fact]
    public void BuildComplexQuery_RealWorldExample()
    {
        // Arrange & Act - Simulate a complex search scenario
        var sieveModel = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterGreaterThanOrEqual(a => a.Createdat, DateTime.Now.AddDays(-30))
            .FilterByName("BooksCount", ">=", 3)
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
        Assert.Equal(1, sieveModel.Page);
        Assert.Equal(20, sieveModel.PageSize);

        _outputHelper.WriteLine($"Complex Query:");
        _outputHelper.WriteLine($"  Filters: {sieveModel.Filters}");
        _outputHelper.WriteLine($"  Sorts: {sieveModel.Sorts}");
        _outputHelper.WriteLine($"  Page: {sieveModel.Page}");
        _outputHelper.WriteLine($"  PageSize: {sieveModel.PageSize}");
    }

    [Fact]
    public void ParseQueryString_ParsesFiltersCorrectly()
    {
        // Arrange
        var queryString = "filters=Name@=Bob,Age>=18&sorts=Name&page=2&pageSize=10";

        // Act
        var builder = SieveQueryBuilder<Author>.ParseQueryString(queryString);
        var filters = builder.GetFilters();

        // Assert
        Assert.Equal(2, filters.Count);
        Assert.Equal("Name", filters[0].PropertyName);
        Assert.Equal("@=", filters[0].Operator);
        Assert.Equal("Bob", filters[0].Value);
        Assert.Equal("Age", filters[1].PropertyName);
        Assert.Equal(">=", filters[1].Operator);
        Assert.Equal("18", filters[1].Value);

        _outputHelper.WriteLine($"Parsed {filters.Count} filters:");
        foreach (var filter in filters)
        {
            _outputHelper.WriteLine($"  {filter.PropertyName} {filter.Operator} {filter.Value}");
        }
    }

    [Fact]
    public void ParseQueryString_ParsesSortsCorrectly()
    {
        // Arrange
        var queryString = "filters=Name@=Bob&sorts=-CreatedAt,Name&page=1&pageSize=20";

        // Act
        var builder = SieveQueryBuilder<Author>.ParseQueryString(queryString);
        var sorts = builder.GetSorts();

        // Assert
        Assert.Equal(2, sorts.Count);
        Assert.Equal("CreatedAt", sorts[0].PropertyName);
        Assert.True(sorts[0].IsDescending);
        Assert.Equal("Name", sorts[1].PropertyName);
        Assert.False(sorts[1].IsDescending);

        _outputHelper.WriteLine($"Parsed {sorts.Count} sorts:");
        foreach (var sort in sorts)
        {
            _outputHelper.WriteLine($"  {sort.PropertyName} ({(sort.IsDescending ? "DESC" : "ASC")})");
        }
    }

    [Fact]
    public void ParseQueryString_ParsesPaginationCorrectly()
    {
        // Arrange
        var queryString = "filters=Name@=Bob&sorts=Name&page=5&pageSize=25";

        // Act
        var builder = SieveQueryBuilder<Author>.ParseQueryString(queryString);

        // Assert
        Assert.Equal(5, builder.GetPage());
        Assert.Equal(25, builder.GetPageSize());

        _outputHelper.WriteLine($"Page: {builder.GetPage()}, PageSize: {builder.GetPageSize()}");
    }

    [Fact]
    public void ParseQueryString_WithUrlEncoding_DecodesCorrectly()
    {
        // Arrange
        var queryString = "filters=Name%40%3DBob%2CAge%3E%3D18&sorts=Name";

        // Act
        var builder = SieveQueryBuilder<Author>.ParseQueryString(queryString);
        var filters = builder.GetFilters();

        // Assert
        Assert.Equal(2, filters.Count);
        Assert.Equal("Name@=Bob", filters[0].OriginalFilter);
        Assert.Equal("Age>=18", filters[1].OriginalFilter);

        _outputHelper.WriteLine($"Decoded filters: {builder.BuildFiltersString()}");
    }

    [Fact]
    public void ParseQueryString_WithLeadingQuestionMark_ParsesCorrectly()
    {
        // Arrange
        var queryString = "?filters=Name@=Bob&page=1";

        // Act
        var builder = SieveQueryBuilder<Author>.ParseQueryString(queryString);

        // Assert
        Assert.Single(builder.GetFilters());
        Assert.Equal(1, builder.GetPage());

        _outputHelper.WriteLine($"Parsed with leading '?': {builder.BuildFiltersString()}");
    }

    [Fact]
    public void FromSieveModel_CreatesBuilderCorrectly()
    {
        // Arrange
        var model = new Sieve.Models.SieveModel
        {
            Filters = "Name@=Bob,Age>=18",
            Sorts = "-CreatedAt,Name",
            Page = 3,
            PageSize = 15
        };

        // Act
        var builder = SieveQueryBuilder<Author>.FromSieveModel(model);
        var filters = builder.GetFilters();
        var sorts = builder.GetSorts();

        // Assert
        Assert.Equal(2, filters.Count);
        Assert.Equal(2, sorts.Count);
        Assert.Equal(3, builder.GetPage());
        Assert.Equal(15, builder.GetPageSize());

        _outputHelper.WriteLine($"FromSieveModel:");
        _outputHelper.WriteLine($"  Filters: {builder.BuildFiltersString()}");
        _outputHelper.WriteLine($"  Sorts: {builder.BuildSortsString()}");
    }

    [Fact]
    public void ParseAndModify_RoundTripWorks()
    {
        // Arrange - Start with a query string
        var originalQuery = "filters=Name@=Bob&sorts=Name&page=1&pageSize=10";

        // Act - Parse, modify, and rebuild
        var builder = SieveQueryBuilder<Author>.ParseQueryString(originalQuery);
        builder.FilterGreaterThanOrEqual(a => a.Createdat, DateTime.Now.AddDays(-7))
               .SortByDescending(a => a.Createdat)
               .Page(2);

        var newQueryString = builder.BuildQueryString();
        var filters = builder.GetFilters();
        var sorts = builder.GetSorts();

        // Assert
        Assert.Equal(2, filters.Count); // Original + new filter
        Assert.Equal(2, sorts.Count);   // Original + new sort
        Assert.Equal(2, builder.GetPage());
        Assert.Equal(10, builder.GetPageSize());

        _outputHelper.WriteLine($"Original: {originalQuery}");
        _outputHelper.WriteLine($"Modified: {newQueryString}");
        _outputHelper.WriteLine($"Filters: {string.Join(", ", filters.Select(f => f.OriginalFilter))}");
        _outputHelper.WriteLine($"Sorts: {string.Join(", ", sorts.Select(s => s.OriginalSort))}");
    }

    [Fact]
    public void HasFilter_DetectsFilterCorrectly()
    {
        // Arrange
        var builder = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterGreaterThanOrEqual(a => a.Createdat, DateTime.Now);

        // Act & Assert
        Assert.True(builder.HasFilter("Name"));
        Assert.True(builder.HasFilter("Createdat"));
        Assert.False(builder.HasFilter("Age"));

        _outputHelper.WriteLine($"Has 'Name' filter: {builder.HasFilter("Name")}");
        _outputHelper.WriteLine($"Has 'Age' filter: {builder.HasFilter("Age")}");
    }

    [Fact]
    public void HasSort_DetectsSortCorrectly()
    {
        // Arrange
        var builder = SieveQueryBuilder<Author>.Create()
            .SortBy(a => a.Name)
            .SortByDescending(a => a.Createdat);

        // Act & Assert
        Assert.True(builder.HasSort("Name"));
        Assert.True(builder.HasSort("Createdat"));
        Assert.False(builder.HasSort("Age"));

        _outputHelper.WriteLine($"Has 'Name' sort: {builder.HasSort("Name")}");
        _outputHelper.WriteLine($"Has 'Age' sort: {builder.HasSort("Age")}");
    }

    [Fact]
    public void GetFilters_ParsesAllOperatorsCorrectly()
    {
        // Arrange
        var builder = SieveQueryBuilder<Author>.Create()
            .FilterEquals(a => a.Name, "Bob")
            .FilterNotEquals(a => a.Name, "Alice")
            .FilterContains(a => a.Name, "test")
            .FilterStartsWith(a => a.Name, "B")
            .FilterGreaterThan(a => a.Name, "A")
            .FilterLessThan(a => a.Name, "Z")
            .FilterGreaterThanOrEqual(a => a.Name, "B")
            .FilterLessThanOrEqual(a => a.Name, "Y");

        // Act
        var filters = builder.GetFilters();

        // Assert
        Assert.Equal(8, filters.Count);
        Assert.Equal("==", filters[0].Operator);
        Assert.Equal("!=", filters[1].Operator);
        Assert.Equal("@=", filters[2].Operator);
        Assert.Equal("_=", filters[3].Operator);
        Assert.Equal(">", filters[4].Operator);
        Assert.Equal("<", filters[5].Operator);
        Assert.Equal(">=", filters[6].Operator);
        Assert.Equal("<=", filters[7].Operator);

        _outputHelper.WriteLine("All operators parsed:");
        foreach (var filter in filters)
        {
            _outputHelper.WriteLine($"  {filter.Operator}: {filter.OriginalFilter}");
        }
    }

    [Fact]
    public void ParseQueryString_EmptyString_ReturnsEmptyBuilder()
    {
        // Arrange & Act
        var builder = SieveQueryBuilder<Author>.ParseQueryString("");
        var filters = builder.GetFilters();
        var sorts = builder.GetSorts();

        // Assert
        Assert.Empty(filters);
        Assert.Empty(sorts);
        Assert.Null(builder.GetPage());
        Assert.Null(builder.GetPageSize());

        _outputHelper.WriteLine("Empty query string creates empty builder");
    }

    [Fact]
    public void FromSieveModel_NullProperties_HandlesGracefully()
    {
        // Arrange
        var model = new Sieve.Models.SieveModel
        {
            Filters = null,
            Sorts = null,
            Page = null,
            PageSize = null
        };

        // Act
        var builder = SieveQueryBuilder<Author>.FromSieveModel(model);

        // Assert
        Assert.Empty(builder.GetFilters());
        Assert.Empty(builder.GetSorts());
        Assert.Null(builder.GetPage());
        Assert.Null(builder.GetPageSize());

        _outputHelper.WriteLine("Null SieveModel properties handled gracefully");
    }
}
