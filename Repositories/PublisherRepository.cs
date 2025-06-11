using BookStoreApi.Data;
using BookStoreApi.Entities;
using BookStoreApi.Extra;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Repositories
{
    public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
    {
        private readonly ApplicationDbContext _context;

        public PublisherRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync(PublisherQueryObject query)
        {
            var publishers = _context.Publishers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                publishers = publishers.Where(p => p.Name.ToLower().Contains(query.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLowerInvariant())
                {
                    case "name":
                        publishers = query.IsDescending ? publishers.OrderByDescending(p => p.Name) : publishers.OrderBy(p => p.Name);
                        break;
                    default:
                        publishers = publishers.OrderBy(p => p.Id);
                        break;
                }
            }
            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            publishers = publishers.Skip(skipNumber).Take(query.PageSize);

            return await publishers.ToListAsync();
        }
    


        // You might need to override GetByIdAsync if it needs to include related data (like Books)
        // public override async Task<Publisher?> GetByIdAsync(int id)
        // {
        //     return await _dbSet.Include(p => p.Books).FirstOrDefaultAsync(p => p.Id == id);
        // }
    }
}
