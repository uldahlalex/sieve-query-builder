/**
 * Type-safe Sieve query string builder for TypeScript
 *
 * Supports building filter, sort, and pagination parameters for Sieve-compatible APIs
 */

/**
 * Represents the Sieve model structure
 */
export interface SieveModel {
  filters?: string;
  sorts?: string;
  page?: number;
  pageSize?: number;
}

/**
 * Extract property keys from a type (excluding functions and symbols)
 */
type PropertyKeys<T> = {
  [K in keyof T]: T[K] extends Function ? never : K;
}[keyof T];

/**
 * Type-safe Sieve query builder
 * @template T The entity type to build queries for
 */
export class SieveQueryBuilder<T extends object> {
  private filters: string[] = [];
  private sorts: string[] = [];
  private pageValue?: number;
  private pageSizeValue?: number;

  /**
   * Create a new SieveQueryBuilder instance
   */
  static create<T extends object>(): SieveQueryBuilder<T> {
    return new SieveQueryBuilder<T>();
  }

  /**
   * Add a filter using equals operator (==)
   */
  filterEquals<K extends PropertyKeys<T>>(
    property: K,
    value: T[K] | string | number | boolean
  ): this {
    this.filters.push(`${String(property)}==${value}`);
    return this;
  }

  /**
   * Add a filter using not equals operator (!=)
   */
  filterNotEquals<K extends PropertyKeys<T>>(
    property: K,
    value: T[K] | string | number | boolean
  ): this {
    this.filters.push(`${String(property)}!=${value}`);
    return this;
  }

  /**
   * Add a filter using contains operator (@=)
   */
  filterContains<K extends PropertyKeys<T>>(
    property: K,
    value: string
  ): this {
    this.filters.push(`${String(property)}@=${value}`);
    return this;
  }

  /**
   * Add a filter using starts with operator (_=)
   */
  filterStartsWith<K extends PropertyKeys<T>>(
    property: K,
    value: string
  ): this {
    this.filters.push(`${String(property)}_=${value}`);
    return this;
  }

  /**
   * Add a filter using greater than operator (>)
   */
  filterGreaterThan<K extends PropertyKeys<T>>(
    property: K,
    value: T[K] | string | number | Date
  ): this {
    const formattedValue = value instanceof Date ? value.toISOString() : value;
    this.filters.push(`${String(property)}>${formattedValue}`);
    return this;
  }

  /**
   * Add a filter using less than operator (<)
   */
  filterLessThan<K extends PropertyKeys<T>>(
    property: K,
    value: T[K] | string | number | Date
  ): this {
    const formattedValue = value instanceof Date ? value.toISOString() : value;
    this.filters.push(`${String(property)}<${formattedValue}`);
    return this;
  }

  /**
   * Add a filter using greater than or equal operator (>=)
   */
  filterGreaterThanOrEqual<K extends PropertyKeys<T>>(
    property: K,
    value: T[K] | string | number | Date
  ): this {
    const formattedValue = value instanceof Date ? value.toISOString() : value;
    this.filters.push(`${String(property)}>=${formattedValue}`);
    return this;
  }

  /**
   * Add a filter using less than or equal operator (<=)
   */
  filterLessThanOrEqual<K extends PropertyKeys<T>>(
    property: K,
    value: T[K] | string | number | Date
  ): this {
    const formattedValue = value instanceof Date ? value.toISOString() : value;
    this.filters.push(`${String(property)}<=${formattedValue}`);
    return this;
  }

  /**
   * Add a filter using a custom property name (for mapped properties)
   * @param propertyName The custom property name (e.g., "BooksCount")
   * @param operator The operator symbol (e.g., ">=", "==", "@=")
   * @param value The value to filter by
   */
  filterByName(propertyName: string, operator: string, value: string | number | boolean | Date): this {
    const formattedValue = value instanceof Date ? value.toISOString() : value;
    this.filters.push(`${propertyName}${operator}${formattedValue}`);
    return this;
  }

  /**
   * Add ascending sort for a property
   */
  sortBy<K extends PropertyKeys<T>>(property: K): this {
    this.sorts.push(String(property));
    return this;
  }

  /**
   * Add descending sort for a property
   */
  sortByDescending<K extends PropertyKeys<T>>(property: K): this {
    this.sorts.push(`-${String(property)}`);
    return this;
  }

  /**
   * Add sort using a custom property name (for mapped properties)
   * @param propertyName The custom property name (e.g., "BooksCount")
   * @param descending Whether to sort descending (default: false)
   */
  sortByName(propertyName: string, descending: boolean = false): this {
    this.sorts.push(descending ? `-${propertyName}` : propertyName);
    return this;
  }

  /**
   * Set the page number for pagination
   */
  page(page: number): this {
    this.pageValue = page;
    return this;
  }

  /**
   * Set the page size for pagination
   */
  pageSize(pageSize: number): this {
    this.pageSizeValue = pageSize;
    return this;
  }

  /**
   * Build the Filters query string component
   */
  buildFiltersString(): string {
    return this.filters.join(',');
  }

  /**
   * Build the Sorts query string component
   */
  buildSortsString(): string {
    return this.sorts.join(',');
  }

  /**
   * Build a complete SieveModel object
   */
  buildSieveModel(): SieveModel {
    const model: SieveModel = {};

    if (this.filters.length > 0) {
      model.filters = this.buildFiltersString();
    }

    if (this.sorts.length > 0) {
      model.sorts = this.buildSortsString();
    }

    if (this.pageValue !== undefined) {
      model.page = this.pageValue;
    }

    if (this.pageSizeValue !== undefined) {
      model.pageSize = this.pageSizeValue;
    }

    return model;
  }

  /**
   * Build the complete query string for use in HTTP requests
   */
  buildQueryString(): string {
    const parts: string[] = [];

    if (this.filters.length > 0) {
      parts.push(`filters=${encodeURIComponent(this.filters.join(','))}`);
    }

    if (this.sorts.length > 0) {
      parts.push(`sorts=${encodeURIComponent(this.sorts.join(','))}`);
    }

    if (this.pageValue !== undefined) {
      parts.push(`page=${this.pageValue}`);
    }

    if (this.pageSizeValue !== undefined) {
      parts.push(`pageSize=${this.pageSizeValue}`);
    }

    return parts.join('&');
  }

  /**
   * Build an object suitable for use as query parameters in fetch/axios
   */
  buildQueryParams(): Record<string, string | number> {
    const params: Record<string, string | number> = {};

    if (this.filters.length > 0) {
      params.filters = this.filters.join(',');
    }

    if (this.sorts.length > 0) {
      params.sorts = this.sorts.join(',');
    }

    if (this.pageValue !== undefined) {
      params.page = this.pageValue;
    }

    if (this.pageSizeValue !== undefined) {
      params.pageSize = this.pageSizeValue;
    }

    return params;
  }
}
