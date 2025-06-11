using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync(AuthorQueryObject query);
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<Author> CreateAuthorAsync(Author author);
        Task<bool> UpdateAuthorAsync(Author author);
        Task<bool> DeleteAuthorAsync(Author author);
        Task SaveChangesAsync();
    }
}
