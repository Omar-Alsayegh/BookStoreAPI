
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BookStoreApi.Data;
using System.Reflection;

namespace BookStoreApi.Repositories
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
           var entry= await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            //var entityToDelete = await _dbSet.FindAsync(id);
            //if (entityToDelete != null)
            //{

             _dbSet.Remove(entity);
            //}
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var property = Expression.Property(parameter, "Id"); // Assumes property is named "Id"
            var constant = Expression.Constant(id);
            var body = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            return await _dbSet.AnyAsync(lambda);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Entry(entity).State = EntityState.Modified;
           // return Task.CompletedTask;
            return Task.FromResult(entity);
        }
    }
}
