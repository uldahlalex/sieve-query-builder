# TypeScript Sieve Query Builder

A type-safe query string builder for Sieve-compatible APIs in TypeScript. Build complex filtering, sorting, and pagination queries with full IntelliSense support.

## Features

- **Type-safe**: Full TypeScript support with compile-time property name checking
- **Fluent API**: Chain methods for readable query building
- **Multiple output formats**: Query strings, SieveModel objects, or query parameter objects
- **All Sieve operators**: `==`, `!=`, `@=` (contains), `_=` (starts with), `>`, `<`, `>=`, `<=`
- **Custom property support**: Handle mapped properties from your Sieve processor
- **Date handling**: Automatic ISO string conversion for Date objects
- **Zero dependencies**: Lightweight and standalone

## Installation

```bash
npm install ts-sieve-query-builder
```

## Usage

### Basic Example

```typescript
import { SieveQueryBuilder } from 'ts-sieve-query-builder';

// Define your entity interface (or use generated types from NSwag)
interface Author {
  id: string;
  name: string;
  createdat: Date;
}

// Build a type-safe query
const sieveModel = SieveQueryBuilder.create<Author>()
  .filterContains('name', 'Bob')
  .sortBy('name')
  .pageSize(10)
  .buildSieveModel();

// Result: { filters: "name@=Bob", sorts: "name", pageSize: 10 }
```


### Filtering Operations

```typescript
const builder = SieveQueryBuilder.create<Author>();

// Equals
builder.filterEquals('name', 'Bob_5');
// Result: "name==Bob_5"

// Not Equals
builder.filterNotEquals('name', 'Bob_0');
// Result: "name!=Bob_0"

// Contains
builder.filterContains('name', 'Bob');
// Result: "name@=Bob"

// Starts With
builder.filterStartsWith('name', 'Bob');
// Result: "name_=Bob"

// Greater Than
builder.filterGreaterThan('pages', 200);
// Result: "pages>200"

// Less Than
builder.filterLessThan('pages', 500);
// Result: "pages<500"

// Greater Than or Equal
builder.filterGreaterThanOrEqual('pages', 200);
// Result: "pages>=200"

// Less Than or Equal
builder.filterLessThanOrEqual('pages', 500);
// Result: "pages<=500"
```

### Date Filtering

```typescript
const thirtyDaysAgo = new Date();
thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);

const query = SieveQueryBuilder.create<Author>()
  .filterGreaterThanOrEqual('createdat', thirtyDaysAgo)
  .buildFiltersString();

// Result: "createdat>=2024-10-14T12:00:00.000Z" (automatically converted to ISO string)
```

### Custom Mapped Properties

For properties mapped in your C# `ApplicationSieveProcessor`:

This feature intentionally bypasses type-safety.

```typescript
// C# Sieve Processor maps a.Books.Count to "BooksCount"
const query = SieveQueryBuilder.create<Author>()
  .filterByName('BooksCount', '>=', 5)
  .sortByName('BooksCount', true) // descending
  .buildSieveModel();

// Result: { filters: "BooksCount>=5", sorts: "-BooksCount" }
```

### Sorting

```typescript
// Ascending sort
builder.sortBy('name');
// Result: "name"

// Descending sort
builder.sortByDescending('createdat');
// Result: "-createdat"

// Multiple sorts
builder
  .sortByDescending('createdat')
  .sortBy('name');
// Result: "-createdat,name"
```

### Pagination

```typescript
const query = SieveQueryBuilder.create<Author>()
  .page(2)
  .pageSize(25)
  .buildSieveModel();

// Result: { page: 2, pageSize: 25 }
```

### Complex Query Example

```typescript
const sieveModel = SieveQueryBuilder.create<Author>()
  .filterContains('name', 'Bob')
  .filterGreaterThanOrEqual('createdat', new Date('2024-01-01'))
  .filterByName('BooksCount', '>=', 3)
  .sortByDescending('createdat')
  .sortBy('name')
  .page(1)
  .pageSize(20)
  .buildSieveModel();

// Result:
// {
//   filters: "name@=Bob,createdat>=2024-01-01T00:00:00.000Z,BooksCount>=3",
//   sorts: "-createdat,name",
//   page: 1,
//   pageSize: 20
// }
```

### Parsing from URL Search Params

You can construct a query builder from existing query parameters (e.g., from URL search params):

```typescript
import { useSearchParams } from 'react-router-dom';

// In your React component
const [searchParams] = useSearchParams();
const filters = searchParams.get('filters') ?? "";
const sorts = searchParams.get('sorts') ?? "";
const pageSize = Number.parseInt(searchParams.get('pageSize') ?? "10");
const page = Number.parseInt(searchParams.get('page') ?? "1");

const queryBuilder = SieveQueryBuilder.parseFromString<Author>({
  pageSize: pageSize,
  page: page,
  sorts: sorts,
  filters: filters
});

// Continue building on top of the parsed query
queryBuilder
  .filterEquals('status', 'active')
  .sortByDescending('createdat');

// Or just use it as-is
const model = queryBuilder.buildSieveModel();
```

### Round-trip Parsing

You can parse a SieveModel back into a builder for modification:

```typescript
// Build a query
const original = SieveQueryBuilder.create<Author>()
  .filterContains('name', 'Bob')
  .sortBy('name')
  .page(1)
  .pageSize(10);

// Convert to model
const model = original.buildSieveModel();

// Parse back to builder and modify
const modified = SieveQueryBuilder.parseFromString<Author>(model)
  .filterGreaterThanOrEqual('createdat', new Date('2024-01-01'))
  .page(2);

const newModel = modified.buildSieveModel();
// Result includes both original and new filters/sorts
```

### Output Formats

#### 1. SieveModel Object

```typescript
const model = builder.buildSieveModel();
// { filters: "name@=Bob", sorts: "name", page: 1, pageSize: 10 }
```

#### 2. Query String

```typescript
const queryString = builder.buildQueryString();
// "filters=name%40%3DBob&sorts=name&page=1&pageSize=10"
```

#### 3. Query Parameters Object

```typescript
const params = builder.buildQueryParams();
// { filters: "name@=Bob", sorts: "name", page: 1, pageSize: 10 }

// Use with URLSearchParams
const url = `/api/authors?${new URLSearchParams(params)}`;
```

#### 4. Individual Components

```typescript
const filtersString = builder.buildFiltersString();
// "name@=Bob,name!=Bob_0"

const sortsString = builder.buildSortsString();
// "-createdat,name"
```


## Testing

The package includes comprehensive tests. Run them with:

```bash
npm test
```

## Browser Compatibility

Works in all modern browsers and Node.js environments that support ES2020.

## License

MIT

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.
