using BookStoreApi.Extra;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Entities;

namespace BookStoreApi.Services
{
    public interface IPublisherService
    {
        Task<IEnumerable<Publisher>> GetAllPublishersAsync(PublisherQueryObject query);
        Task<Publisher?> GetPublisherByIdAsync(int id);
        Task<Publisher> CreatePublisherAsync(Publisher publisher);
        Task<bool> UpdatePublisherAsync(Publisher publisher);
        Task<bool> DeletePublisherAsync(Publisher publisher);
        Task SaveChangesAsync();
    }
}
