using BookStoreApi.Entities;
using BookStoreApi.Extra;

namespace BookStoreApi.Repositories
{
    public interface IPublisherRepository : IGenericRepository<Publisher>
    {
        Task<IEnumerable<Publisher>> GetAllPublishersAsync(PublisherQueryObject query);
    }
}
