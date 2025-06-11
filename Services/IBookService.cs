using BookStoreApi.Extra;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Entities;

namespace BookStoreApi.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(QueryObject query);
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(Book book);
        Task SaveChangesAsync();
    }
}
