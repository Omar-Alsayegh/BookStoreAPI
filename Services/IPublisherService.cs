using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Services
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherDto>> GetAllPublishersAsync();
        Task<PublisherDto?> GetPublisherByIdAsync(int id);
        Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createDto);
        Task<bool> UpdatePublisherAsync(int id, UpdatePublisherDto updateDto);
        Task<bool> DeletePublisherAsync(int id);
    }
}
