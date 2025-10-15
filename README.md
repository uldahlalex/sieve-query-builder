# Sieve.TypeSafeQueryBuilder

Type-safe query builders for [Sieve](https://github.com/Biarity/Sieve) filtering, sorting, and pagination. Available for both .NET and TypeScript.

## Overview

This monorepo contains type-safe query builder implementations for building Sieve-compatible query strings with compile-time safety and IntelliSense support.

### Key Features

- **Type-safe** - Compile-time property name checking
- **Full Sieve operator support** - `==`, `!=`, `@=`, `_=`, `>`, `<`, `>=`, `<=`
- **Fluent API** - Chain methods for readable query construction
- **Multiple output formats** - Query strings, objects, or SieveModel
- **Zero magic strings** - Use lambda expressions (C#) or interface properties (TypeScript)

### Table of cotents 

* [Sieve.TypeSafeQueryBuilder](#sievetypesafequerybuilder)
  * [Overview](#overview)
    * [Key Features](#key-features)
  * [Packages](#packages)
    * [TypeScript](#typescript)
    * [.NET / C#](#net--c)
    * [Full-Stack Type Safety](#full-stack-type-safety)
  * [Contributing](#contributing)
  * [License](#license)
<!-- TOC -->

## Packages

### TypeScript

Interface-based query builder for type-safe Sieve queries in any TypeScript application (React, Vue, Angular, etc...)

**[📖 Full TypeScript Documentation →](./ts/README.md)**

```bash
npm install ts-sieve-query-builder
```

**Quick Example:**
```typescript
import { SieveQueryBuilder } from 'ts-sieve-query-builder';

const queryParams = SieveQueryBuilder.create<Author>()
  .filterContains('name', 'Bob')
  .filterGreaterThanOrEqual('createdat', thirtyDaysAgo)
  .sortByDescending('createdat')
  .page(1)
  .pageSize(20)
  .buildQueryParams();

// API request with simple fetch() 
// although type-safe API calls are preferred
const response = await fetch(`/api/authors?${queryParams}`);

```


### .NET / C#

Due to testing, I have also made a C#/.NET equivalent, so you can perform your .NET unit tests with a typesafe query.


**[📖 Full .NET Documentation →](./dotnet/README.md)**

```bash
dotnet add package Sieve.TypeSafe.QueryBuilder
```

**Quick Example:**
```csharp
var queryString = SieveQueryBuilder<Author>.Create()
    .FilterContains(a => a.Name, "Bob")
    .FilterGreaterThanOrEqual(a => a.CreatedAt, DateTime.Now.AddDays(-7))
    .SortByDescending(a => a.CreatedAt)
    .Page(1)
    .PageSize(20)
    .BuildQueryString();
```

**Query Models** for custom mapped properties:
```csharp
// Define a query model matching your SieveProcessor configuration
public class AuthorQueryModel : ISieveQueryModel
{
    public string? Name { get; set; }
    public int? BooksCount { get; set; }  // Custom property with IntelliSense!
}

// Use it with full type safety
var query = SieveQueryBuilder<AuthorQueryModel>.Create()
    .FilterContains(a => a.Name, "Bob")
    .FilterGreaterThanOrEqual(a => a.BooksCount, 5)  // Type-safe custom property!
    .BuildQueryString();
```

**Generate Query Models** from a single source of truth:
```csharp
// Define configuration once
var builder = new SieveQueryModelBuilder<Author>()
    .AddProperty<string>("Name")
    .AddProperty<int>("BooksCount");

// Generate both query model code AND SieveProcessor configuration
var queryModelCode = builder.GenerateQueryModelCode();     // C# class code
var processorCode = builder.GenerateSieveProcessorCode();  // mapper.Property<Author>(...) code
```

**Round-trip parsing** - parse and modify queries:
```csharp
// Parse incoming queries
var builder = SieveQueryBuilder<Author>
    .ParseQueryString(Request.QueryString.Value);

// Add server-side filters
builder.FilterEquals(a => a.IsDeleted, false);

// Build and apply
var results = _sieveProcessor.Apply(builder.BuildSieveModel(), dbContext.Authors);
```


### Full-Stack Type Safety
Use with to generate TypeScript types from your C# DTOs, then use the same entity types in both the .NET backend and TypeScript frontend query builders.

Links:
- https://github.com/RicoSuter/NSwag/wiki/TypeScriptClientGenerator
- https://github.com/RicoSuter/NSwag 

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for either package.

## License

MIT
