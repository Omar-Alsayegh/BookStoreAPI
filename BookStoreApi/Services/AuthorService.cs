using BookStoreApi.Models.DTOs;
using BookStoreApi.Mappings;
using BookStoreApi.Entities;
using BookStoreApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreApi.Extra;


namespace BookStoreApi.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenericRepository<Author> _genericRepository;

        public AuthorService(IAuthorRepository authorRepository, IGenericRepository<Author> genericRepository)
        {
            _authorRepository = authorRepository;
            _genericRepository= genericRepository;
        }
        public async Task<Author> CreateAuthorAsync(Author newauthor)
        {
           return  await _authorRepository.AddAsync(newauthor);
        }

        public async Task<bool> DeleteAuthorAsync(Author authortoDelete)
        {
            await _authorRepository.DeleteAsync(authortoDelete);
            return true;
        }
        //public async Task<IEnumerable<Author>> GetAllAuthorsAsync(AuthorQueryObject query)
        //{
        //    //return await _authorRepository.GetAllAuthorsAsync(query);
        //    return await _genericRepository.GetAsync(query);
        //}

        public async Task<IEnumerable<Author>> GetFilteredAuthorsAsync(AuthorQueryObject query)
        {
            // 1. Build the filterString
            var filterParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                // Example: Name.ToLower().Contains("john doe") for case-insensitive search
                filterParts.Add($"Name.ToLower().Contains(\"{query.Name.ToLower()}\")");
            }
            if (query.Birthdate.HasValue)
            {
                // For DateTime, use a parseable string format for Dynamic LINQ's DateTime() constructor.
                // ISO 8601 format without time zone specifier or Z is generally safe for equality checks.
                filterParts.Add($"Birthdate = DateTime(\"{query.Birthdate.Value:yyyy-MM-ddTHH:mm:ss.fffffff}\")");
                // For ranges: filterParts.Add($"Birthdate >= DateTime(\"{query.Birthdate.Value:yyyy-MM-ddTHH:mm:ss.fffffff}\")");
            }

            string? filterString = filterParts.Any() ? string.Join(" AND ", filterParts) : null;


            // 2. Build the orderByString
            string? orderByString = null;
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                string sortBy = query.SortBy; // Use the property name directly
                string direction = query.IsDescending ? "descending" : "ascending";

                // Ensure the SortBy property exists on the Author entity
                // You might want to validate this (e.g., using reflection or a predefined list)
                // to prevent runtime errors if query.SortBy is unexpected.
                orderByString = $"{sortBy} {direction}";
            }
            else
            {
                orderByString = "Id ascending"; // Default sort
            }

            // 3. Build the includeStrings
            // This depends on your Author entity's actual navigation properties.
            // If Author has a many-to-many relationship with Book via BookAuthor:
            string? includeStrings = "BookAuthors.Book"; // Include books through the join table

            // If Author had a direct one-to-many to Books (less common):
            // string? includeStrings = "Books";


            // 4. Calculate pagination parameters
            query.PageNumber = Math.Max(1, query.PageNumber);
            var skipAmount = (query.PageNumber - 1) * query.PageSize;


            // 5. Call the generic repository's GetAsync method
            var authors = await _authorRepository.GetAsync(
                filterString: filterString,
                orderByString: orderByString,
                includeStrings: includeStrings,
                skip: skipAmount,
                take: query.PageSize
            );

           // _logger.LogInformation("Retrieved {Count} authors using filtered query via Dynamic LINQ.", authors.Count());
            return authors;
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _authorRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAuthorAsync(Author existingAuthor)
        {
            await _authorRepository.UpdateAsync(existingAuthor);
            return true;
        }
        public async Task SaveChangesAsync()
        {
            await _authorRepository.SaveChangesAsync();
        }
    }
}
