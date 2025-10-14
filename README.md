# Sieve.TypeSafeQueryBuilder

A type-safe query builder for [Sieve](https://github.com/Biarity/Sieve) with round-trip parsing support. Build, parse, and inspect Sieve filters, sorts, and pagination with compile-time safety.

## Features

- **Type-safe query building** - Use lambda expressions instead of magic strings
- **Round-trip parsing** - Parse query strings and SieveModels back to builders
- **Inspection API** - Examine filters and sorts programmatically
- **Full operator support** - All Sieve operators (==, !=, @=, _=, >, <, >=, <=)
- **Fluent API** - Chain methods for readable query construction
- **Compatible with any Sieve version** - Flexible dependency on Sieve 1.0.0+

## Installation

```bash
dotnet add package Sieve.TypeSafeQueryBuilder
```

## Quick Start

### Building Queries

```csharp
using src;

// Type-safe query building
var queryString = SieveQueryBuilder<Author>.Create()
    .FilterContains(a => a.Name, "Bob")
    .FilterGreaterThanOrEqual(a => a.CreatedAt, DateTime.Now.AddDays(-7))
    .SortByDescending(a => a.CreatedAt)
    .SortBy(a => a.Name)
    .Page(1)
    .PageSize(20)
    .BuildQueryString();

// Result: "filters=Name@=Bob,CreatedAt>=2024-01-07&sorts=-CreatedAt,Name&page=1&pageSize=20"
```

### Building SieveModel

```csharp
var sieveModel = SieveQueryBuilder<Author>.Create()
    .FilterEquals(a => a.Status, "Active")
    .SortBy(a => a.Name)
    .Page(1)
    .PageSize(25)
    .BuildSieveModel();

// Use with Sieve
var results = sieveProcessor.Apply(sieveModel, query);
```

## Round-Trip Parsing

### Parse Query Strings

```csharp
// Parse incoming HTTP query string
var builder = SieveQueryBuilder<Author>
    .ParseQueryString("filters=Name@=Bob,Age>=18&sorts=-CreatedAt&page=2");

// Inspect what was parsed
var filters = builder.GetFilters();
foreach (var filter in filters)
{
    Console.WriteLine($"{filter.PropertyName} {filter.Operator} {filter.Value}");
}
// Output:
// Name @= Bob
// Age >= 18
```

### Parse from SieveModel

```csharp
// Parse from existing SieveModel
var builder = SieveQueryBuilder<Author>.FromSieveModel(existingSieveModel);

// Add more filters type-safely
builder.FilterEquals(a => a.IsActive, true)
       .SortBy(a => a.Email);

// Rebuild
var newSieveModel = builder.BuildSieveModel();
```

## Inspection API

```csharp
var builder = SieveQueryBuilder<Author>.ParseQueryString(queryString);

// Get structured filter information
var filters = builder.GetFilters();
foreach (var filter in filters)
{
    Console.WriteLine($"Property: {filter.PropertyName}");
    Console.WriteLine($"Operator: {filter.Operator}");
    Console.WriteLine($"Value: {filter.Value}");
}

// Get structured sort information
var sorts = builder.GetSorts();
foreach (var sort in sorts)
{
    Console.WriteLine($"{sort.PropertyName} ({(sort.IsDescending ? "DESC" : "ASC")})");
}

// Check for specific filters/sorts
if (builder.HasFilter("Name"))
{
    Console.WriteLine("Has name filter");
}

// Get pagination
var page = builder.GetPage();
var pageSize = builder.GetPageSize();
```

## All Filter Operators

```csharp
var builder = SieveQueryBuilder<Author>.Create();

builder.FilterEquals(a => a.Name, "Bob");              // Name==Bob
builder.FilterNotEquals(a => a.Name, "Alice");         // Name!=Alice
builder.FilterContains(a => a.Name, "test");           // Name@=test
builder.FilterStartsWith(a => a.Name, "B");            // Name_=B
builder.FilterGreaterThan(a => a.Age, 18);             // Age>18
builder.FilterLessThan(a => a.Age, 65);                // Age<65
builder.FilterGreaterThanOrEqual(a => a.Age, 18);      // Age>=18
builder.FilterLessThanOrEqual(a => a.Age, 65);         // Age<=65

// For custom/mapped properties
builder.FilterByName("BooksCount", ">=", 5);           // BooksCount>=5
```

## Sorting

```csharp
var builder = SieveQueryBuilder<Author>.Create();

builder.SortBy(a => a.Name);                           // Name
builder.SortByDescending(a => a.CreatedAt);            // -CreatedAt

// For custom/mapped properties
builder.SortByName("BooksCount", descending: true);    // -BooksCount

// Multiple sorts
builder.SortByDescending(a => a.CreatedAt)
       .SortBy(a => a.Name);                           // -CreatedAt,Name
```

## Real-World Example

```csharp
// API Controller
[HttpGet]
public async Task<IActionResult> GetAuthors([FromQuery] string filters, [FromQuery] string sorts)
{
    // Parse incoming query
    var queryBuilder = SieveQueryBuilder<Author>
        .ParseQueryString($"filters={filters}&sorts={sorts}");

    // Add server-side filters
    queryBuilder.FilterEquals(a => a.IsDeleted, false);

    // Check if user requested sensitive data
    if (queryBuilder.HasFilter("Email") && !User.IsAdmin())
    {
        return Forbid();
    }

    // Build SieveModel
    var sieveModel = queryBuilder.BuildSieveModel();

    // Apply with Sieve
    var authors = await _sieveProcessor
        .Apply(sieveModel, _context.Authors)
        .ToListAsync();

    return Ok(authors);
}
```

## Output Formats

### Query String
```csharp
var queryString = builder.BuildQueryString();
// "filters=Name@=Bob&sorts=-CreatedAt&page=1&pageSize=20"
```

### SieveModel
```csharp
var sieveModel = builder.BuildSieveModel();
// SieveModel { Filters = "Name@=Bob", Sorts = "-CreatedAt", Page = 1, PageSize = 20 }
```

### Individual Components
```csharp
var filtersString = builder.BuildFiltersString();  // "Name@=Bob,Age>=18"
var sortsString = builder.BuildSortsString();      // "-CreatedAt,Name"
```

## Version Compatibility

This package is compatible with **any version of Sieve >= 1.0.0**. The dependency uses a wildcard version to maximize compatibility with consumer projects.

## License

MIT

## Contributing

Contributions welcome! Please open an issue or PR on [GitHub](https://github.com/yourusername/sievetypesafequerybuilder).
