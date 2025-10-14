import { describe, it, expect } from 'vitest';
import { SieveQueryBuilder } from './SieveQueryBuilder';

// Sample interfaces matching your C# entities
interface Author {
  id: string;
  name: string;
  createdat: Date;
  books: Book[];
}

interface Book {
  id: string;
  title: string;
  pages: number;
  createdat: Date;
  authors: Author[];
}

describe('SieveQueryBuilder', () => {
  describe('Filter operations', () => {
    it('should build equals filter', () => {
      const query = SieveQueryBuilder.create<Author>()
        .filterEquals('name', 'Bob_5')
        .buildFiltersString();

      expect(query).toBe('name==Bob_5');
    });

    it('should build contains filter', () => {
      const query = SieveQueryBuilder.create<Author>()
        .filterContains('name', '_5')
        .buildFiltersString();

      expect(query).toBe('name@=_5');
    });

    it('should build not equals filter', () => {
      const query = SieveQueryBuilder.create<Author>()
        .filterNotEquals('name', 'Bob_0')
        .buildFiltersString();

      expect(query).toBe('name!=Bob_0');
    });

    it('should build starts with filter', () => {
      const query = SieveQueryBuilder.create<Author>()
        .filterStartsWith('name', 'Bob')
        .buildFiltersString();

      expect(query).toBe('name_=Bob');
    });

    it('should build greater than filter', () => {
      const query = SieveQueryBuilder.create<Book>()
        .filterGreaterThan('pages', 200)
        .buildFiltersString();

      expect(query).toBe('pages>200');
    });

    it('should build less than filter', () => {
      const query = SieveQueryBuilder.create<Book>()
        .filterLessThan('pages', 500)
        .buildFiltersString();

      expect(query).toBe('pages<500');
    });

    it('should build greater than or equal filter', () => {
      const query = SieveQueryBuilder.create<Book>()
        .filterGreaterThanOrEqual('pages', 200)
        .buildFiltersString();

      expect(query).toBe('pages>=200');
    });

    it('should build less than or equal filter', () => {
      const query = SieveQueryBuilder.create<Book>()
        .filterLessThanOrEqual('pages', 500)
        .buildFiltersString();

      expect(query).toBe('pages<=500');
    });

    it('should combine multiple filters', () => {
      const query = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob')
        .filterNotEquals('name', 'Bob_0')
        .buildFiltersString();

      expect(query).toBe('name@=Bob,name!=Bob_0');
    });

    it('should build custom property filter', () => {
      const query = SieveQueryBuilder.create<Author>()
        .filterByName('BooksCount', '>=', 5)
        .buildFiltersString();

      expect(query).toBe('BooksCount>=5');
    });

    it('should handle Date filters', () => {
      const date = new Date('2024-01-01T00:00:00Z');
      const query = SieveQueryBuilder.create<Author>()
        .filterGreaterThanOrEqual('createdat', date)
        .buildFiltersString();

      expect(query).toBe('createdat>=2024-01-01T00:00:00.000Z');
    });
  });

  describe('Sort operations', () => {
    it('should build ascending sort', () => {
      const query = SieveQueryBuilder.create<Author>()
        .sortBy('name')
        .buildSortsString();

      expect(query).toBe('name');
    });

    it('should build descending sort', () => {
      const query = SieveQueryBuilder.create<Author>()
        .sortByDescending('name')
        .buildSortsString();

      expect(query).toBe('-name');
    });

    it('should combine multiple sorts', () => {
      const query = SieveQueryBuilder.create<Author>()
        .sortByDescending('createdat')
        .sortBy('name')
        .buildSortsString();

      expect(query).toBe('-createdat,name');
    });

    it('should build custom property sort', () => {
      const query = SieveQueryBuilder.create<Author>()
        .sortByName('BooksCount', true)
        .buildSortsString();

      expect(query).toBe('-BooksCount');
    });
  });

  describe('Pagination', () => {
    it('should set page number', () => {
      const model = SieveQueryBuilder.create<Author>()
        .page(2)
        .buildSieveModel();

      expect(model.page).toBe(2);
    });

    it('should set page size', () => {
      const model = SieveQueryBuilder.create<Author>()
        .pageSize(10)
        .buildSieveModel();

      expect(model.pageSize).toBe(10);
    });
  });

  describe('SieveModel building', () => {
    it('should build complete SieveModel', () => {
      const model = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob')
        .sortBy('name')
        .page(2)
        .pageSize(10)
        .buildSieveModel();

      expect(model).toEqual({
        filters: 'name@=Bob',
        sorts: 'name',
        page: 2,
        pageSize: 10,
      });
    });

    it('should only include non-empty values in SieveModel', () => {
      const model = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob')
        .buildSieveModel();

      expect(model).toEqual({
        filters: 'name@=Bob',
      });
      expect(model.sorts).toBeUndefined();
      expect(model.page).toBeUndefined();
      expect(model.pageSize).toBeUndefined();
    });
  });

  describe('Query string building', () => {
    it('should build complete query string', () => {
      const queryString = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob')
        .filterNotEquals('name', 'Bob_0')
        .sortBy('name')
        .page(2)
        .pageSize(10)
        .buildQueryString();

      expect(queryString).toBe(
        'filters=name%40%3DBob%2Cname%21%3DBob_0&sorts=name&page=2&pageSize=10'
      );
    });

    it('should handle empty query string', () => {
      const queryString = SieveQueryBuilder.create<Author>().buildQueryString();

      expect(queryString).toBe('');
    });
  });

  describe('Query params building', () => {
    it('should build query params object', () => {
      const params = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob')
        .sortBy('name')
        .page(1)
        .pageSize(20)
        .buildQueryParams();

      expect(params).toEqual({
        filters: 'name@=Bob',
        sorts: 'name',
        page: 1,
        pageSize: 20,
      });
    });

    it('should return empty object when no params', () => {
      const params = SieveQueryBuilder.create<Author>().buildQueryParams();

      expect(params).toEqual({});
    });
  });

  describe('Fluent API chaining', () => {
    it('should support method chaining', () => {
      const query = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob')
        .filterGreaterThanOrEqual('createdat', new Date('2024-01-01'))
        .filterByName('BooksCount', '>=', 3)
        .sortByDescending('createdat')
        .sortBy('name')
        .page(1)
        .pageSize(20);

      const model = query.buildSieveModel();

      expect(model.filters).toContain('name@=Bob');
      expect(model.filters).toContain('createdat>=');
      expect(model.filters).toContain('BooksCount>=3');
      expect(model.sorts).toBe('-createdat,name');
      expect(model.page).toBe(1);
      expect(model.pageSize).toBe(20);
    });
  });

  describe('Type safety', () => {
    it('should only allow valid property names', () => {
      const builder = SieveQueryBuilder.create<Author>();

      // These should compile without errors
      builder.filterEquals('name', 'test');
      builder.filterEquals('id', 'test');
      builder.sortBy('createdat');

      // @ts-expect-error - 'invalidProperty' does not exist on Author
      builder.filterEquals('invalidProperty', 'test');
    });
  });

  describe('Real-world usage examples', () => {
    it('should build query for filtering authors by name contains', () => {
      const queryString = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob_0')
        .pageSize(10)
        .buildQueryString();

      expect(queryString).toBe('filters=name%40%3DBob_0&pageSize=10');
    });

    it('should build query for complex search scenario', () => {
      const thirtyDaysAgo = new Date();
      thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);

      const model = SieveQueryBuilder.create<Author>()
        .filterContains('name', 'Bob')
        .filterGreaterThanOrEqual('createdat', thirtyDaysAgo)
        .filterByName('BooksCount', '>=', 3)
        .sortByDescending('createdat')
        .sortBy('name')
        .page(1)
        .pageSize(20)
        .buildSieveModel();

      expect(model.filters).toBeDefined();
      expect(model.filters).toContain('name@=Bob');
      expect(model.filters).toContain('BooksCount>=3');
      expect(model.sorts).toBe('-createdat,name');
      expect(model.page).toBe(1);
      expect(model.pageSize).toBe(20);
    });

    it('should build query for books with page count filtering', () => {
      const params = SieveQueryBuilder.create<Book>()
        .filterGreaterThan('pages', 200)
        .filterLessThan('pages', 500)
        .sortBy('title')
        .buildQueryParams();

      expect(params.filters).toBe('pages>200,pages<500');
      expect(params.sorts).toBe('title');
    });
  });
});
