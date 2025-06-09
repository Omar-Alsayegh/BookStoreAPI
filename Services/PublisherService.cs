using BookStoreApi.Models.DTOs;
using BookStoreApi.Repositories;
using BookStoreApi.Mappings;
using BookStoreApi.Extensions;

namespace BookStoreApi.Services
{

    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository;

        public PublisherService(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createDto)
        {
            var newPublisher = createDto.ToEntity();
            await _publisherRepository.AddAsync(newPublisher);
            await _publisherRepository.SaveChangesAsync();
            return newPublisher.ToDto();
        }

        public async Task<bool> DeletePublisherAsync(int id)
        {
            var booktoDelete = _publisherRepository.GetByIdAsync(id);
            if (booktoDelete != null)
            {
                return false;
            }
            await _publisherRepository.DeleteAsync(id);
            await _publisherRepository.SaveChangesAsync();
            return true;

        }

        public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync()
        {
            var Publishers = await _publisherRepository.GetAllAsync();
            return Publishers.Select(x => x.ToDto()).ToList();
        }

        public async Task<PublisherDto?> GetPublisherByIdAsync(int id)
        {
            var publisher = await _publisherRepository.GetByIdAsync(id);
            return publisher?.ToDto();
        }

        public async Task<bool> UpdatePublisherAsync(int id, UpdatePublisherDto updateDto)
        {
            var updatedPublisher = await _publisherRepository.GetByIdAsync(id);
            if (updatedPublisher != null)
            {
                return false;
            }
            updatedPublisher.UpdateFromDto(updateDto);
            await _publisherRepository.UpdateAsync(updatedPublisher);
            await _publisherRepository.SaveChangesAsync();
            return true;

        }
    }
}
