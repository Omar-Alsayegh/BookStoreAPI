using BookStoreApi.Entities;
using BookStoreApi.Extra;

namespace BookStoreApi.Repositories
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {

        Task<IEnumerable<Author>> GetAllAuthorsAsync(AuthorQueryObject query);
    }
}
