using BookStoreApi.Extra;
using System.Linq.Expressions;

namespace BookStoreApi.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<bool> ExistsAsync(int id); // To check if an entity exists before update/delete
        Task SaveChangesAsync(); // To commit changes to the database

        Task<IEnumerable<TEntity>> GetAsync(
            string? filterString = null,
            string? orderByString = null,
            string? includeStrings = null,
            int? skip = null,
            int? take = null);

    }
}
