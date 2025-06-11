using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Repositories;

namespace BookStoreApi.Services
{

    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository;

        public PublisherService(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        public async Task<Publisher> CreatePublisherAsync(Publisher newpublisher)
        {
            return await _publisherRepository.AddAsync(newpublisher);
        }

        public async Task<bool> DeletePublisherAsync(Publisher publisher)
        {
            await _publisherRepository.DeleteAsync(publisher);
            return true;

        }

        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync(PublisherQueryObject query)
        {
            return await _publisherRepository.GetAllPublishersAsync(query);
        }

        public async Task<Publisher?> GetPublisherByIdAsync(int id)
        {
            return await _publisherRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdatePublisherAsync(Publisher publisher)
        {
            await _publisherRepository.UpdateAsync(publisher);
            return true;
        }
        public async Task SaveChangesAsync()
        {
            await _publisherRepository.SaveChangesAsync();
        }
    }
}
