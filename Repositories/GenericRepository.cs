
using BookStoreApi.Data;
using BookStoreApi.Entities;
using BookStoreApi.Extra;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace BookStoreApi.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
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
            return Task.FromResult(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAsync(
             string? filterString = null,
             string? orderByString = null,
             string? includeStrings = null,
             int? skip = null,
             int? take = null)
        {
            IQueryable<TEntity> query = _dbSet;

            // 1. Apply Includes (eager loading of related entities)
            if (!string.IsNullOrWhiteSpace(includeStrings))
            {
                var includes = includeStrings.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                foreach (var include in includes)
                {
                    query = query.Include(include); // Uses EF Core's string-based Include
                }
            }

            // 2. Apply Filter (WHERE clause) using Dynamic LINQ
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                query = query.Where(filterString); // Uses System.Linq.Dynamic.Core's Where extension
            }

            // 3. Apply OrderBy (ORDER BY clause) using Dynamic LINQ
            if (!string.IsNullOrWhiteSpace(orderByString))
            {
                query = query.OrderBy(orderByString); // Uses System.Linq.Dynamic.Core's OrderBy extension
            }
       
            // 4. Apply Pagination (SKIP and TAKE)
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            // 5. Execute the query against the database and materialize the results
            return await query.ToListAsync();
        }


    }
}
