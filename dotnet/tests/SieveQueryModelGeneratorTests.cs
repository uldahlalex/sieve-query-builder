using SieveQueryBuilder;
using tests.Entities;

namespace tests;

/// <summary>
/// Tests for query model builder - manual configuration approach
/// </summary>
public class SieveQueryModelGeneratorTests
{
    private readonly ITestOutputHelper _outputHelper;

    public SieveQueryModelGeneratorTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void ModelBuilder_ConfiguresPropertiesForAuthor()
    {
        // Arrange & Act - Manually configure properties (single source of truth)
        var builder = new SieveQueryModelBuilder<Author>()
            .AddProperty<string>("Id")
            .AddProperty<string>("Name")
            .AddProperty<DateTime>("Createdat")
            .AddProperty<int>("BooksCount");  // Custom mapped property

        var properties = builder.GetProperties();

        // Assert
        Assert.Equal(4, properties.Count);

        var propertyNames = properties.Select(p => p.PropertyName).ToList();
        Assert.Contains("Id", propertyNames);
        Assert.Contains("Name", propertyNames);
        Assert.Contains("Createdat", propertyNames);
        Assert.Contains("BooksCount", propertyNames);

        _outputHelper.WriteLine($"Configured {properties.Count} properties for Author:");
        foreach (var prop in properties)
        {
            _outputHelper.WriteLine($"  - {prop.PropertyName} (Type: {prop.PropertyType.Name}, Filter: {prop.CanFilter}, Sort: {prop.CanSort})");
        }
    }

    [Fact]
    public void ModelBuilder_GeneratesValidQueryModelCode()
    {
        // Arrange
        var builder = new SieveQueryModelBuilder<Author>()
            .AddProperty<string>("Id")
            .AddProperty<string>("Name")
            .AddProperty<DateTime>("Createdat")
            .AddProperty<int>("BooksCount");

        // Act
        var code = builder.GenerateQueryModelCode();

        // Assert
        Assert.Contains("public class AuthorQueryModel : ISieveQueryModel", code);
        Assert.Contains("public string? Id { get; set; }", code);
        Assert.Contains("public string? Name { get; set; }", code);
        Assert.Contains("public DateTime? Createdat { get; set; }", code);
        Assert.Contains("public int? BooksCount { get; set; }", code);

        _outputHelper.WriteLine("Generated Query Model Code:");
        _outputHelper.WriteLine(code);
    }

    [Fact]
    public void ModelBuilder_GeneratesSieveProcessorCode()
    {
        // Arrange
        var builder = new SieveQueryModelBuilder<Author>()
            .AddProperty<string>("Id")
            .AddProperty<string>("Name")
            .AddProperty<DateTime>("Createdat")
            .AddProperty<int>("BooksCount");

        // Act
        var code = builder.GenerateSieveProcessorCode();

        // Assert
        Assert.Contains("mapper.Property<Author>(e => e.Id)", code);
        Assert.Contains("mapper.Property<Author>(e => e.Name)", code);
        Assert.Contains(".CanFilter()", code);
        Assert.Contains(".CanSort()", code);

        _outputHelper.WriteLine("Generated SieveProcessor Code:");
        _outputHelper.WriteLine(code);
    }

    [Fact]
    public void ModelBuilder_AllowsCustomModelName()
    {
        // Arrange
        var builder = new SieveQueryModelBuilder<Author>()
            .AddProperty<string>("Name");

        // Act
        var code = builder.GenerateQueryModelCode("CustomAuthorQuery");

        // Assert
        Assert.Contains("public class CustomAuthorQuery : ISieveQueryModel", code);
        Assert.DoesNotContain("AuthorQueryModel", code);

        _outputHelper.WriteLine("Generated code with custom name:");
        _outputHelper.WriteLine(code);
    }

    [Fact]
    public void ModelBuilder_SupportsFilterOnlyProperties()
    {
        // Arrange & Act
        var builder = new SieveQueryModelBuilder<Author>()
            .AddProperty<string>("Name", canSort: false);  // Filter only

        var properties = builder.GetProperties();

        // Assert
        var nameProp = properties.First();
        Assert.True(nameProp.CanFilter);
        Assert.False(nameProp.CanSort);

        _outputHelper.WriteLine($"Name property: Filter={nameProp.CanFilter}, Sort={nameProp.CanSort}");
    }

    [Fact]
    public void ModelBuilder_SupportsSortOnlyProperties()
    {
        // Arrange & Act
        var builder = new SieveQueryModelBuilder<Author>()
            .AddProperty<string>("Name", canFilter: false);  // Sort only

        var properties = builder.GetProperties();

        // Assert
        var nameProp = properties.First();
        Assert.False(nameProp.CanFilter);
        Assert.True(nameProp.CanSort);

        _outputHelper.WriteLine($"Name property: Filter={nameProp.CanFilter}, Sort={nameProp.CanSort}");
    }

    [Fact]
    public void ModelBuilder_GeneratesCodeForBookEntity()
    {
        // Arrange
        var builder = new SieveQueryModelBuilder<Book>()
            .AddProperty<string>("Id")
            .AddProperty<string>("Title")
            .AddProperty<int>("Pages")
            .AddProperty<DateTime>("Createdat");

        // Act
        var code = builder.GenerateQueryModelCode();

        // Assert
        Assert.Contains("public class BookQueryModel : ISieveQueryModel", code);
        Assert.Contains("public string? Id { get; set; }", code);
        Assert.Contains("public string? Title { get; set; }", code);
        Assert.Contains("public int? Pages { get; set; }", code);

        _outputHelper.WriteLine("Generated BookQueryModel Code:");
        _outputHelper.WriteLine(code);
    }

    [Fact]
    public void ModelBuilder_FullWorkflowExample()
    {
        // Arrange - Single source of truth for configuration
        var builder = new SieveQueryModelBuilder<Author>()
            .AddProperty<string>("Id")
            .AddProperty<string>("Name")
            .AddProperty<DateTime>("Createdat")
            .AddProperty<int>("BooksCount");  // Custom mapped property

        // Act - Generate both query model and processor code
        var queryModelCode = builder.GenerateQueryModelCode();
        var processorCode = builder.GenerateSieveProcessorCode();

        // Assert
        Assert.Contains("AuthorQueryModel", queryModelCode);
        Assert.Contains("BooksCount", queryModelCode);
        Assert.Contains("mapper.Property<Author>", processorCode);
        Assert.Contains("BooksCount", processorCode);

        _outputHelper.WriteLine("=== Query Model Code ===");
        _outputHelper.WriteLine(queryModelCode);
        _outputHelper.WriteLine("");
        _outputHelper.WriteLine("=== SieveProcessor Code ===");
        _outputHelper.WriteLine(processorCode);
    }
}
