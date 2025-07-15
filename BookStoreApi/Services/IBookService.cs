using BookStoreApi.Extra;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Entities;

namespace BookStoreApi.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(QueryObject query);
        Task<Book> GetBookWithAuthorsAsync(string bookTitle);
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(Book book);
        Task<IEnumerable<Book>> GetBooksAddedSinceAsync(DateTimeOffset sinceDate);
        Task<BookImage> GetBookImageAsync(int PhotoId);
        Task<IEnumerable<Book>> GetFilteredBooksAsync(QueryObject query);
        Task SaveChangesAsync();
    }
}
