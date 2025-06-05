using BookStoreApi.Data;
using BookStoreApi.Entities;

namespace BookStoreApi.Repositories
{
    public class AuthorRepository: GenericRepository<Author>, IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
