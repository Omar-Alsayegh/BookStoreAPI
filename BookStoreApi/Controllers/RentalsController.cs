using BookStoreApi.Data.Data;
using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Models;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RentalsController> _logger;
        private readonly IRentalService _rentalServicess;

        public RentalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<RentalsController> logger,IRentalService servicess)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _rentalServicess = servicess;
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetAllRentals([FromQuery] RentalQueryObject query)
        {
            var rentalsQuery = _context.Rentals
            .Include(r => r.Book)
            .Include(r => r.User)
            .AsQueryable();
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
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                string sortByLower = query.SortBy.ToLowerInvariant();

                if (sortByLower.Contains("rentaldate") || sortByLower.Contains("date"))
                {
                    rentalsQuery = query.IsDescending ? rentalsQuery.OrderByDescending(r => r.RentalDate) : rentalsQuery.OrderBy(r => r.RentalDate);
                }
                else if (sortByLower.Contains("booktitle") || sortByLower.Contains("title"))
                {
                    rentalsQuery = query.IsDescending
                        ? rentalsQuery.OrderByDescending(r => r.Book != null ? r.Book.Title : string.Empty)
                        : rentalsQuery.OrderBy(r => r.Book != null ? r.Book.Title : string.Empty);
                }
                else if (sortByLower.Contains("customeremail") || sortByLower.Contains("email"))
                {
                    rentalsQuery = query.IsDescending
                        ? rentalsQuery.OrderByDescending(r => r.User != null && r.User.Email != null ? r.User.Email : string.Empty)
                        : rentalsQuery.OrderBy(r => r.User != null && r.User.Email != null ? r.User.Email : string.Empty);
                }
                else if (sortByLower.Contains("status"))
                {
                    rentalsQuery = query.IsDescending ? rentalsQuery.OrderByDescending(r => r.Status) : rentalsQuery.OrderBy(r => r.Status);
                }
                else
                {
                    rentalsQuery = rentalsQuery.OrderByDescending(r => r.RentalDate); 
                }
            }
            else
            {
                rentalsQuery = rentalsQuery.OrderByDescending(r => r.RentalDate);
            }

            query.PageNumber = Math.Max(1, query.PageNumber);

            var skipAmount = (query.PageNumber - 1) * query.PageSize;

            rentalsQuery = rentalsQuery
                .Skip(skipAmount)
                .Take(query.PageSize);


            var rentals = await _context.Rentals
                .Include(r => r.Book)
                .Include(r => r.User)
                .OrderByDescending(r => r.RentalDate)
                .Select(r => new RentalDto
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    BookTitle = r.Book!.Title,
                    CustomerEmail = r.User!.Email!,
                    RentalDate = r.RentalDate,
                    ExpectedReturnDate = r.ExpectedReturnDate,
                    ActualReturnDate = r.ActualReturnDate,
                    ApprovedBy = r.ApprovedBy,
                    ReasonOfRejection = r.ReasonOfRejection,
                    Status = r.Status
                }).ToListAsync();
            _logger.LogInformation("Retrieved {Count} all rentals by Admin.", rentals.Count);
            return Ok(rentals);
        }


        [Authorize (Roles ="Admin,Employee,Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetRentalsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("GetMyRentals failed: User ID not found in claims for authenticated user.");
                return Unauthorized("User ID not found.");
            }
            var rentals = await _context.Rentals
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .OrderByDescending(r => r.RentalDate)
                .Select(r => new RentalDto
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    BookTitle = r.Book!.Title,
                    CustomerEmail = r.User!.Email!,
                    RentalDate = r.RentalDate,
                    ExpectedReturnDate = r.ExpectedReturnDate,
                    ActualReturnDate = r.ActualReturnDate,
                    ApprovedBy = r.ApprovedBy,
                    ReasonOfRejection = r.ReasonOfRejection,
                    Status = r.Status
                })
                .ToListAsync();
            _logger.LogInformation("Retrieved {Count} rentals for User ID {UserId}.", rentals.Count, userId);
            return Ok(rentals);
        }

        [Authorize(Roles = "Admin,Employee,Customer")]
        [HttpPost]
        public async Task<IActionResult> AddRentalAsync([FromBody] RentalRequestDto request) { 
            var user= User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                _logger.LogInformation("Adding a rental to a Not Found User");
                return NotFound("user not found");
            }
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
            {
                _logger.LogInformation("Book is not found in the database!!");
                return NotFound("Book not found");
            }
            if (book.StockQuantity == 0)
            {
                _logger.LogInformation("The stock quantity of {book} is 0 so book is not available to be rented",request.BookId);
                return NotFound("Book is out of stock");
            }

            var hasActiveRental = await _context.Rentals
         .AnyAsync(r => r.UserId == user &&
                        (r.Status == RentalStatus.Pending || r.Status == RentalStatus.Accepted));

            if (hasActiveRental)
            {
                _logger.LogInformation("User {UserId} already has an active rental and cannot rent again.", user);
                return Conflict("You already have an active rental and cannot rent another book at this time.");
            }
            var days=request.DesiredDurationDays;
            var time = _rentalServicess.FixTime(DateTime.UtcNow, days);
            var rental = new Rental
            {
                BookId = request.BookId,
                UserId = user,
                RentalDate = DateTime.UtcNow,
                ApprovedBy = null,
                Status = RentalStatus.Pending,
                ExpectedReturnDate =time,
                CreatedBy = user,    
                ModifiedBy = user,
                CreatedAt = DateTime.UtcNow, 
                ModifiedAt = DateTime.UtcNow
            };
            _context.Rentals.Add(rental);
            _context.SaveChanges();

            _logger.LogInformation("New rental request created for Book ID {BookId} by User ID {UserId}. Rental ID: {RentalId}", request.BookId, user, rental.Id);
            return Accepted(new { Message = "Rental request submitted. Awaiting approval.", RentalId = rental.Id });
           
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> RentalStatusResponse([FromQuery] int id, [FromBody] RentalStatusUpdateRequestDto response)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                _logger.LogInformation("AcceptRental failed: User {User} not found.",user);
                return NotFound("User not found");
            }

            var rental = await _context.Rentals
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (rental == null)
            {
                _logger.LogInformation("AcceptRental failed: Rental with ID {RentalId} not found.", id);
                return NotFound("Rental not found");
            }
            if (rental.Status != RentalStatus.Pending)
            {
                _logger.LogWarning("AcceptRental failed for Rental ID {RentalId}: Status is {Status}, not Pending.", id, rental.Status);
                return BadRequest($"Rental is not in 'Pending' status. Current status: {rental.Status}.");
            }
            if (rental.Book == null || rental.Book.StockQuantity == 0)
            {
                _logger.LogInformation("AcceptRental failed: The book with Id {Bookid} is out of stock or not found.", id);
                return NotFound("Book not found");
            }
            if (response.rentalStatus == RentalStatus.Accepted)
            {
                rental.Status = RentalStatus.Accepted;
                rental.Book.StockQuantity--;
                rental.ApprovedBy = user;
                rental.ModifiedBy = user;
                rental.CreatedBy = user;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Rental ID {RentalId} for Book '{BookTitle}' accepted by {AcceptorRole}. New stock: {NewStock}.",
                                   id, rental.Book.Title, User.Identity!.Name, rental.Book.StockQuantity);

                return Ok($"Rental ID {id} accepted. Book '{rental.Book.Title}' now active.");
            }
            else
            {
                rental.Status = RentalStatus.Rejected;
                rental.ApprovedBy = "Not Approved By "+user;
                rental.ModifiedBy = user;
                rental.CreatedBy = user;
                rental.ReasonOfRejection =response.ReasonOfRejection;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Rental Rejection success");
                return Ok("Rental Rejected");
            }
        }
        [HttpPut("End")]
        [Authorize (Roles ="Admin,Employee")]
        public async Task<IActionResult> EndRental([FromQuery] int id,[FromQuery] DateTime ReturnDate )
        {
            var rental = await _context.Rentals
                .Include(r => r.Book)           
                .FirstOrDefaultAsync(r => r.Id==id);

            if (rental == null)
            {
                _logger.LogWarning("EndRental failed: Rental with ID {RentalId} not found.", id);
                return NotFound("Rental not found.");
            }

            if (rental.Status != RentalStatus.Accepted && rental.Status != RentalStatus.Overdue)
            {
                _logger.LogWarning("EndRental failed for Rental ID {RentalId}: Status is {Status}, not Active or Overdue.", id, rental.Status);
                return BadRequest($"Rental is not in 'Accepted' or 'Overdue' status. Current status: {rental.Status}.");
            }
            if (rental.Book != null)
            {
                rental.Book.StockQuantity++;
            }
            else
            {
                _logger.LogError("EndRental for Rental ID {RentalId} failed: Associated book is null. Cannot increment stock.", id);
            }

            rental.ActualReturnDate = DateTime.UtcNow;
            rental.Status = RentalStatus.Returned;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Rental ID {RentalId} for Book '{BookTitle}' ended by {EnderRole}. New stock: {NewStock}.",
                                   id, rental.Book?.Title ?? "N/A", User.Identity!.Name, rental.Book?.StockQuantity ?? -1);

            return Ok($"Rental ID {id} ended. Book '{rental.Book?.Title ?? "N/A"}' marked as returned.");
        }

    }
}
