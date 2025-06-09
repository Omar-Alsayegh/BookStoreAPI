using BookStoreApi.Data;
using BookStoreApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _dbSet
                         .Include(b => b.BookAuthors) // Include the join entity
                             .ThenInclude(ba => ba.Author) // Then include the Author from the join entity
                         .Include(b => b.Publisher) // Include the Publisher directly
                         .ToListAsync();
        }

        // Override GetByIdAsync to include Author and Publisher for a single book
        public override async Task<Book?> GetByIdAsync(int id)
        {
            return await _dbSet
                         .Include(b => b.BookAuthors)
                             .ThenInclude(ba => ba.Author)
                         .Include(b => b.Publisher)
                         .FirstOrDefaultAsync(b => b.Id == id); // Find by ID using FirstOrDefaultAsync
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName)
        {
            return await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Publisher)
                .Where(b => b.BookAuthors.Any(ba => ba.Author.Name == authorName))
                .ToListAsync();
        }

        //   public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName)
        //{
        //    return await _context.Books.Include(b=> b.Author)
        //        .Where(b=>b.Author.Name == authorName)
        //        .ToListAsync();
        //}


    }
}
