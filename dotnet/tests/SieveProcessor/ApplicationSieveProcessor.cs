using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using tests.Entities;

namespace tests;

/// <summary>
/// Application-specific Sieve processor with property mappings
/// </summary>
public class ApplicationSieveProcessor : SieveProcessor
{
    public ApplicationSieveProcessor(IOptions<SieveOptions> options) : base(options)
    {
    }

    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        // Configure Author entity
        mapper.Property<Author>(a => a.Id)
            .CanFilter()
            .CanSort();

        mapper.Property<Author>(a => a.Name)
            .CanFilter()
            .CanSort();

        mapper.Property<Author>(a => a.Createdat)
            .CanFilter()
            .CanSort();

        // Custom mapped property - Books.Count as "BooksCount"
        mapper.Property<Author>(a => a.Books.Count)
            .CanFilter()
            .CanSort()
            .HasName("BooksCount");

        // Configure Book entity
        mapper.Property<Book>(b => b.Id)
            .CanFilter()
            .CanSort();

        mapper.Property<Book>(b => b.Title)
            .CanFilter()
            .CanSort();

        mapper.Property<Book>(b => b.Pages)
            .CanFilter()
            .CanSort();

        mapper.Property<Book>(b => b.Createdat)
            .CanFilter()
            .CanSort();

        // Configure Genre entity
        mapper.Property<Genre>(g => g.Id)
            .CanFilter()
            .CanSort();

        mapper.Property<Genre>(g => g.Name)
            .CanFilter()
            .CanSort();

        mapper.Property<Genre>(g => g.Createdat)
            .CanFilter()
            .CanSort();

        return mapper;
    }
}
