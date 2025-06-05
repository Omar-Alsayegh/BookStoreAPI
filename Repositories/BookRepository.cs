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

           public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName)
        {
            return await _context.Books.Include(b=> b.Author)
                .Where(b=>b.Author.Name == authorName)
                .ToListAsync();
        }
    
    
    }
}
