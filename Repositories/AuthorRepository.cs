using BookStoreApi.Data;
using BookStoreApi.Entities;

namespace BookStoreApi.Repositories
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {

        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
