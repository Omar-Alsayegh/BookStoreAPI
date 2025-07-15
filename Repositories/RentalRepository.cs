using Azure.Core;
using BookStoreApi.Data;
using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Repositories
{
    public class RentalRepository
    {
        private readonly ApplicationDbContext _context; // Keep context for specific queries
        private readonly ILogger<RentalRepository> _logger;

        public RentalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsWithDetailsAsync(RentalQueryObject query)
        {
            var rentalsQuery = _context.Rentals
                .Include(r => r.Book)
                .Include(r => r.User)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query.BookTitle))
            {
                rentalsQuery = rentalsQuery.Where(r => r.Book != null && r.Book.Title.ToLower().Contains(query.BookTitle.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(query.CustomerEmail))
            {
                rentalsQuery = rentalsQuery.Where(r => r.User != null && r.User.Email != null && r.User.Email.ToLower().Contains(query.CustomerEmail.ToLower()));
            }
            if (query.Status.HasValue)
            {
                rentalsQuery = rentalsQuery.Where(r => r.Status == query.Status.Value);
            }
            if (query.RentalDateFrom.HasValue)
            {
                rentalsQuery = rentalsQuery.Where(r => r.RentalDate >= query.RentalDateFrom.Value);
            }
            if (query.RentalDateTo.HasValue)
            {
                rentalsQuery = rentalsQuery.Where(r => r.RentalDate <= query.RentalDateTo.Value);
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                string sortByLower = query.SortBy.ToLowerInvariant();
                rentalsQuery = sortByLower switch
                {
                    "rentaldate" or "date" => query.IsDescending ? rentalsQuery.OrderByDescending(r => r.RentalDate) : rentalsQuery.OrderBy(r => r.RentalDate),
                    "booktitle" or "title" => query.IsDescending ? rentalsQuery.OrderByDescending(r => r.Book != null ? r.Book.Title : string.Empty) : rentalsQuery.OrderBy(r => r.Book != null ? r.Book.Title : string.Empty),
                    "customeremail" or "email" => query.IsDescending ? rentalsQuery.OrderByDescending(r => r.User != null && r.User.Email != null ? r.User.Email : string.Empty) : rentalsQuery.OrderBy(r => r.User != null && r.User.Email != null ? r.User.Email : string.Empty),
                    "status" => query.IsDescending ? rentalsQuery.OrderByDescending(r => r.Status) : rentalsQuery.OrderBy(r => r.Status),
                    _ => rentalsQuery.OrderByDescending(r => r.RentalDate), // Default sort
                };
            }
            else
            {
                rentalsQuery = rentalsQuery.OrderByDescending(r => r.RentalDate); // Default sort if no sortBy
            }

            // Apply pagination
            var skipAmount = (query.PageNumber - 1) * query.PageSize;
            rentalsQuery = rentalsQuery.Skip(skipAmount).Take(query.PageSize);

            return await rentalsQuery.ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetRentalsForUserWithDetailsAsync(string userId)
        {
            return await _context.Rentals
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .OrderByDescending(r => r.RentalDate)
                .ToListAsync();
        }

        public async Task<Rental?> GetRentalByIdWithDetailsAsync(int rentalId)
        {
            return await _context.Rentals
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == rentalId);
        }

        public async Task<Rental?> GetPendingOrAcceptedRentalByUserId(string userId)
        {
            return await _context.Rentals
                .Where(r => r.UserId == userId &&
                            (r.Status == RentalStatus.Pending || r.Status == RentalStatus.Accepted))
                .FirstOrDefaultAsync();
        }
    }
}
