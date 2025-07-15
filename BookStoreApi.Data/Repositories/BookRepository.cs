using BookStoreApi.Data.Data;
using BookStoreApi.Entities;
using BookStoreApi.Extra;
//using BookStoreApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .ToListAsync();
        }


        public override async Task<Book?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetByIdWithIncludesAsync(int id, string includeProperties, CancellationToken cancellationToken)
        {
            IQueryable<Book> query = _dbSet; // Start with _dbSet (which is DbSet<Book>)

            // Apply includes first
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (string includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty); // Include now works because query is IQueryable<Book>
                }
            }

            // Apply filters once at the end
            // Assuming your Book entity has an 'Id' property (int) and 'InactiveDate' (DateTime?)
            query = query.Where(x => x.Id == id);

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        // Removed: GetBooksByAuthorAsync method as GetFilteredBooksQueryAsync covers its functionality.

        public async Task<IEnumerable<Book>> GetFilteredBooksQueryAsync(QueryObject query)
        {
            var booksQuery = _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .AsQueryable();

            // Apply Filters
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(query.Title.ToLower())); // Added case-insensitive
            }
            if (!string.IsNullOrWhiteSpace(query.AuthorName))
            {
                booksQuery = booksQuery.Where(b => b.BookAuthors.Any(ba => ba.Author.Name.ToLower().Contains(query.AuthorName.ToLower()))); // Added case-insensitive
            }
            if (!string.IsNullOrWhiteSpace(query.PublisherName))
            {
                booksQuery = booksQuery.Where(b => b.Publisher.Name.ToLower().Contains(query.PublisherName.ToLower())); // Added case-insensitive
            }

            // Apply Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                string sortByLower = query.SortBy.ToLowerInvariant();

                if (sortByLower.Contains("title"))
                {
                    booksQuery = query.IsDescending ? booksQuery.OrderByDescending(b => b.Title) : booksQuery.OrderBy(b => b.Title);
                }
                else if (sortByLower.Contains("publicationyear") || sortByLower.Contains("year")) // Added "year" as an alternative
                {
                    booksQuery = query.IsDescending ? booksQuery.OrderByDescending(b => b.PublicationYear) : booksQuery.OrderBy(b => b.PublicationYear);
                }
                else if (sortByLower.Contains("publishername") || sortByLower.Contains("publisher")) // Added "publisher" as an alternative
                {
                    booksQuery = query.IsDescending
                        ? booksQuery.OrderByDescending(b => b.Publisher != null ? b.Publisher.Name : string.Empty)
                        : booksQuery.OrderBy(b => b.Publisher != null ? b.Publisher.Name : string.Empty);
                }
                else
                {
                    // Default sort if no specific match is found for SortBy content
                    booksQuery = booksQuery.OrderBy(b => b.Id);
                }
            }
            else
            {
                booksQuery = booksQuery.OrderBy(b => b.Id);
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            booksQuery = booksQuery.Skip(skipNumber).Take(query.PageSize);

            return await booksQuery.ToListAsync();
        }

        public async Task<Book> GetBookWithAuthorsAsync(string bookTitle)
        {
            return await _context.Books
           .Include(b => b.BookAuthors)
               .ThenInclude(ba => ba.Author)
               .Include(b => b.Publisher)
        .Include(b => b.BookContentPhotos)
           .FirstOrDefaultAsync(b => b.Title == bookTitle);
        }
        public async Task<IEnumerable<Book>> GetBooksAddedSinceAsync(DateTimeOffset sinceDate)
        {
            return await _dbSet
                .Where(b => b.CreatedAt >= sinceDate)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .ToListAsync();
        }
    }
}