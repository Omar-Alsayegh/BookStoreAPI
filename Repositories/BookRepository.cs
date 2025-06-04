using BookStoreApi.Data;
using BookStoreApi.Models;
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
            return await _context.Books.Where(b => b.Author == authorName).ToListAsync();
        }
    
    
    }
}
