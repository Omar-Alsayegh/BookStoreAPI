using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto createDto);
        Task<bool> UpdateBookAsync(int id, UpdateBookDto updateDto); 
        Task<bool> DeleteBookAsync(int id);
    }
}
