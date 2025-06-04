
using BookStoreApi.Models;

namespace BookStoreApi.Repositories
{
    public interface IBookRepository:IGenericRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName);
    }
}
