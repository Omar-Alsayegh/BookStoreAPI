using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Repositories;

namespace BookStoreApi.Services
{

    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository;
        private readonly IGenericRepository<Publisher> _genericRepository;

        public PublisherService(IPublisherRepository publisherRepository, IGenericRepository<Publisher> genericRepository)
        {
            _publisherRepository = publisherRepository;
            _genericRepository = genericRepository;
        }

        public async Task<Publisher> CreatePublisherAsync(Publisher newpublisher)
        {
            return await _publisherRepository.AddAsync(newpublisher);
        }

        public async Task<bool> DeletePublisherAsync(Publisher publisher)
        {
            await _publisherRepository.DeleteAsync(publisher);
            return true;

        }

        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync(PublisherQueryObject query)
        {
            return await _publisherRepository.GetAllPublishersAsync(query);
        }

        public async Task<Publisher?> GetPublisherByIdAsync(int id)
        {
            return await _publisherRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdatePublisherAsync(Publisher publisher)
        {
            await _publisherRepository.UpdateAsync(publisher);
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _publisherRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Publisher>> GetFilteredPublishersAsync(PublisherQueryObject query)
        {
            // 1. Build the filterString
            var filterParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                // Example: Name.ToLower().Contains("some publisher name") for case-insensitive search
                filterParts.Add($"Name.ToLower().Contains(\"{query.Name.ToLower()}\")");
            }
        
            string? filterString = filterParts.Any() ? string.Join(" AND ", filterParts) : null;


            // 2. Build the orderByString
            string? orderByString = null;
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                string sortBy = query.SortBy; // Use the property name (e.g., "Name", "EstablishedYear")
                string direction = query.IsDescending ? "descending" : "ascending";

                // You might want to add validation here to ensure SortBy maps to an actual property
                orderByString = $"{sortBy} {direction}";
            }
            else
            {
                orderByString = "Id ascending"; // Default sort if no sortBy provided
            }

            // 3. Build the includeStrings
            // This depends on your Publisher entity's actual navigation properties.
            // Assuming Publisher has a direct one-to-many relationship with Books:
            string? includeStrings = "Books"; // Include books directly related to the publisher

            // If Publisher has a many-to-many relationship with Book via a join table (e.g., PublisherBook):
            // string? includeStrings = "PublisherBooks.Book";


            // 4. Calculate pagination parameters
            query.PageNumber = Math.Max(1, query.PageNumber);
            var skipAmount = (query.PageNumber - 1) * query.PageSize;


            // 5. Call the generic repository's GetAsync method
            var publishers = await _publisherRepository.GetAsync(
                filterString: filterString,
                orderByString: orderByString,
                includeStrings: includeStrings,
                skip: skipAmount,
                take: query.PageSize
            );

            // Optional: Log information about the retrieved data
           // _logger.LogInformation("Retrieved {Count} publishers using filtered query via Dynamic LINQ.", publishers.Count());

            return publishers;
        }
    }
}
