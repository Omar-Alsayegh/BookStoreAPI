using BookStoreApi.Data;
using BookStoreApi.Entities;

namespace BookStoreApi.Repositories
{
    public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
    {

        public PublisherRepository(ApplicationDbContext context) : base(context)
        {   
        }
    }
}
