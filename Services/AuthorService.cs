using BookStoreApi.Models.DTOs;
using BookStoreApi.Mappings;
using BookStoreApi.Entities;
using BookStoreApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BookStoreApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createDto)
        {
            var newauthor = createDto.CreateToEntity();
            await _authorRepository.AddAsync(newauthor);
            await _authorRepository.SaveChangesAsync();
            return newauthor.ToDto();
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var authortobedel = await _authorRepository.GetByIdAsync(id);
            if (authortobedel == null)
            {
                return false;
            }
            await _authorRepository.DeleteAsync(id);
            await _authorRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            return authors.Select(a => a.ToDto()).ToList();
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            return author?.ToDto();
        }

        public async Task<bool> UpdateAuthorAsync(int id, UpdateAuthorDto updateDto)
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(id);
            if (existingAuthor == null)
            {
                return false;
            }
            existingAuthor.UpdateFromDto(updateDto);
            await _authorRepository.UpdateAsync(existingAuthor);
            await _authorRepository.SaveChangesAsync();
            return true;
        }
    }
}
