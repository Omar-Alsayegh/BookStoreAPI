using BookStoreApi.Extensions;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Repositories;

namespace BookStoreApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            bookRepository = _bookRepository;
        }
        public async Task<BookDto> CreateBookAsync(CreateBookDto createDto)
        {
            var newbook =createDto.CreateToBook();
            await _bookRepository.AddAsync(newbook);
            await _bookRepository.SaveChangesAsync();
            return newbook.ToBookDto();

        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var DelBook= _bookRepository.GetByIdAsync(id);
            if (DelBook != null) { 
                await _bookRepository.DeleteAsync(id);
                await _bookRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books =await _bookRepository.GetAllAsync();
            return books.Select(b => b.ToBookDto());
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book =await _bookRepository.GetByIdAsync(id);
            return book.ToBookDto();
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto updateDto)
        {
            var updatedbook = await _bookRepository.GetByIdAsync(id);
            if (updatedbook != null)
            {
                return false;
            }
            updatedbook.UpdateToBook(updateDto);
            await _bookRepository.UpdateAsync(updatedbook);
            await _bookRepository.SaveChangesAsync();
            return true;

        }
    }
}
