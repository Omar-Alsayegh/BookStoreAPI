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
            //var newauthor = createDto.CreateToEntity();
           return  await _authorRepository.AddAsync(newauthor);
          //  await _authorRepository.SaveChangesAsync();// hay lzm 7ota bl controller
            //return newauthor.ToDto();
        }

        public async Task<bool> DeleteAuthorAsync(Author authortoDelete)
        {
            //var authortobedel = await _authorRepository.GetByIdAsync(id);
            //if (authortobedel == null)
            //{
            //    return false;
            //}
            await _authorRepository.DeleteAsync(authortoDelete);
            //await _authorRepository.SaveChangesAsync();
            return true;
        }

        //public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        //{
        //    var authors = await _authorRepository.GetAllAsync();
        //    return authors.Select(a => a.ToDto()).ToList();
        //}
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
           // var existingAuthor = await _authorRepository.GetByIdAsync(id);
            //if (existingAuthor == null)
            //{
            //    return false;
            //}
            //existingAuthor.UpdateFromDto(updateDto);
            await _authorRepository.UpdateAsync(existingAuthor);
           // await _authorRepository.SaveChangesAsync();
            return true;
        }
        public async Task SaveChangesAsync()
        {
            await _authorRepository.SaveChangesAsync();
        }
    }
}
