using BookStoreApi.Models.DTOs;
using BookStoreApi.Mappings;
using BookStoreApi.Entities;
using BookStoreApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreApi.Extra;


namespace BookStoreApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        public async Task<Author> CreateAuthorAsync(Author newauthor)
        {
           return  await _authorRepository.AddAsync(newauthor);
        }

        public async Task<bool> DeleteAuthorAsync(Author authortoDelete)
        {
            await _authorRepository.DeleteAsync(authortoDelete);
            return true;
        }
        public async Task<IEnumerable<Author>> GetAllAuthorsAsync(AuthorQueryObject query)
        {
           return await _authorRepository.GetAllAuthorsAsync(query);
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _authorRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAuthorAsync(Author existingAuthor)
        {
            await _authorRepository.UpdateAsync(existingAuthor);
            return true;
        }
        public async Task SaveChangesAsync()
        {
            await _authorRepository.SaveChangesAsync();
        }
    }
}
