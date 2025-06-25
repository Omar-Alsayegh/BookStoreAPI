using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IPublisherRepository _publisherRepository;
        public BookService(IBookRepository bookRepository, IPublisherRepository publisherRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
        }
        public async Task<Book> CreateBookAsync(Book book)
        {
            return await _bookRepository.AddAsync(book);

        }

        public async Task<bool> DeleteBookAsync(Book book)
        {
            await _bookRepository.DeleteAsync(book);
            return true;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(QueryObject query)
        {
            return await _bookRepository.GetFilteredBooksQueryAsync(query);

        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
            return true;

        }

        public async Task SaveChangesAsync()
        {
            await _bookRepository.SaveChangesAsync();
        }

        public async Task<Book>GetBookWithAuthorsAsync(string bookTitle)
        {
            return await _bookRepository.GetBookWithAuthorsAsync(bookTitle);
        }
    }
}
