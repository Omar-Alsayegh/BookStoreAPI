using BookStoreApi.Data;
using BookStoreApi.Entities;

namespace BookStoreApi.Repositories
{
    public class PublisherRepository:GenericRepository<Publisher>, IPublisherRepository
    {
        private readonly ApplicationDbContext _context;

        public PublisherRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
