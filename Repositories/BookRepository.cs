using BookStoreApi.Data;
using BookStoreApi.Entities;
using BookStoreApi.Extra;
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
                switch (query.SortBy.ToLowerInvariant()) // Changed to Invariant for consistency
                {
                    case "title":
                        booksQuery = query.IsDescending ? booksQuery.OrderByDescending(b => b.Title) : booksQuery.OrderBy(b => b.Title);
                        break;
                    case "publicationyear":
                        booksQuery = query.IsDescending ? booksQuery.OrderByDescending(b => b.PublicationYear) : booksQuery.OrderBy(b => b.PublicationYear);
                        break;
                    case "publishername":
                        booksQuery = query.IsDescending ? booksQuery.OrderByDescending(b => b.Publisher.Name) : booksQuery.OrderBy(b => b.Publisher.Name);
                        break;
                    default:
                        booksQuery = booksQuery.OrderBy(b => b.Id);
                        break;
                }
            }
            else
            {
                booksQuery = booksQuery.OrderBy(b => b.Id);
            }

            // Apply Pagination - CRITICAL FIX!
            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            booksQuery = booksQuery.Skip(skipNumber).Take(query.PageSize);

            return await booksQuery.ToListAsync();
        }

        public async Task<Book> GetBookWithAuthorsAsync(string bookTitle)
        {
            return await _context.Books
           .Include(b => b.BookAuthors)
               .ThenInclude(ba => ba.Author)
           .FirstOrDefaultAsync(b => b.Title == bookTitle);
        }
    }
}