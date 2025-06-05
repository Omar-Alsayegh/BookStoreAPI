using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
        Task<AuthorDto?> GetAuthorByIdAsync(int id);
        Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createDto);
        Task<bool> UpdateAuthorAsync(int id, UpdateAuthorDto updateDto);
        Task<bool> DeleteAuthorAsync(int id);
    }
}
