using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using BookStoreApi.Mappings;

namespace BookStoreApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly ILogger<BookService> _logger;
        private readonly IGenericRepository<BookImage> _bookImageRepository;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IPublisherRepository publisherRepository, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _publisherRepository = publisherRepository;
            _logger = logger;
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            return await _bookRepository.AddAsync(book);

        }

        public async Task<bool> DeleteBookAsync(Book book)
        {
            await _bookRepository.DeleteAsync(book);
            return true;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(QueryObject query)
        {
            return await _bookRepository.GetFilteredBooksQueryAsync(query);

        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
            return true;

        }

        public async Task SaveChangesAsync()
        {
            await _bookRepository.SaveChangesAsync();
        }

        public async Task<Book>GetBookWithAuthorsAsync(string bookTitle)
        {
            return await _bookRepository.GetBookWithAuthorsAsync(bookTitle);
        }

        public async Task<IEnumerable<Book>> GetBooksAddedSinceAsync(DateTimeOffset sinceDate)
        {
            _logger.LogInformation("Service: Fetching books added since {SinceDate} for new arrivals processing.", sinceDate);
            var newBooksEntities = await _bookRepository.GetBooksAddedSinceAsync(sinceDate);
            return newBooksEntities;
        }

        public async Task<BookImage> GetBookImageAsync(int PhotoId)
        {
            return await _bookImageRepository.GetByIdAsync(PhotoId);
        }

        public async Task<IEnumerable<Book>> GetFilteredBooksAsync(QueryObject query)
        {
            // 1. Build the filterString
            var filterParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                filterParts.Add($"Title.ToLower().Contains(\"{query.Title.ToLower()}\")");
            }

            string? filterString = filterParts.Any() ? string.Join(" AND ", filterParts) : null;


            // 2. Build the orderByString
            string? orderByString = null;
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                string sortBy = query.SortBy; // Use the property name (e.g., "Title", "PublicationYear")
                string direction = query.IsDescending ? "descending" : "ascending";

                // You might want to add validation here to ensure SortBy maps to an actual property
                orderByString = $"{sortBy} {direction}";
            }
            else
            {
                orderByString = "Id ascending"; // Default sort if no sortBy provided
            }

            // 3. Build the includeStrings
            // This depends on your Book entity's actual navigation properties.
            // Assuming Book has a direct one-to-many relationship with Publisher and
            // a many-to-many with Author via BookAuthors:
            string? includeStrings = "Publisher,BookAuthors.Author"; // Include Publisher and Authors via join table


            // 4. Calculate pagination parameters
            query.PageNumber = Math.Max(1, query.PageNumber);
            var skipAmount = (query.PageNumber - 1) * query.PageSize;


            // 5. Call the generic repository's GetAsync method with the constructed strings
            var books = await _bookRepository.GetAsync(
                filterString: filterString,
                orderByString: orderByString,
                includeStrings: includeStrings,
                skip: skipAmount,
                take: query.PageSize
            );

            // Optional: Log information about the retrieved data
            _logger.LogInformation("Retrieved {Count} books using filtered query via Dynamic LINQ.", books.Count());

            return books;
        }
    }
}
