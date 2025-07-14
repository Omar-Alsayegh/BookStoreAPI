using BookStoreApi.Data;
using BookStoreApi.Entities;
using BookStoreApi.Extra;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Repositories
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        private readonly ApplicationDbContext _context;
        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
            _context=context;
        }

        //public async Task<IEnumerable<Author>> GetAllAuthorsAsync(AuthorQueryObject query)
        //{
        //    var authors = _context.Authors.AsQueryable();
        //    if (!string.IsNullOrWhiteSpace(query.Name))
        //    {
        //        authors = authors.Where(a => a.Name.ToLower().Contains(query.Name.ToLower()));
        //    }
        //    if (!string.IsNullOrWhiteSpace(query.SortBy))
        //    {
        //        string sortByLower = query.SortBy.ToLowerInvariant();
        //        if (sortByLower.Contains("name")) 
        //        {
        //            authors = query.IsDescending ? authors.OrderByDescending(a => a.Name) : authors.OrderBy(a => a.Name);
        //        }
        //    }

        //    var skipNumber = (query.PageNumber - 1) * query.PageSize;

        //    return await authors.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        //}
    }
}
