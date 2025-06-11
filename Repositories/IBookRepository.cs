
using BookStoreApi.Entities;
using BookStoreApi.Extra;

namespace BookStoreApi.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<IEnumerable<Book>> GetFilteredBooksQueryAsync(QueryObject query);

        // Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName);
    }
}
