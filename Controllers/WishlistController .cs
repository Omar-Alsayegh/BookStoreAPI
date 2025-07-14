using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs.Request;
using BookStoreApi.Models.DTOs.Response;
using BookStoreApi.Repositories;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService; 
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<WishlistItemDto>>> GetUserWishlist()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or user ID not found.");
            }

            try
            {
                var wishlistEntities = await (await _wishlistService.GetQueryableAsync())
                    .Where(wl => wl.ApplicationUserId == userId)
                    .Include(wl => wl.Book) 
                        .ThenInclude(b => b.BookAuthors)
                            .ThenInclude(ba => ba.Author) 
                    .Include(wl => wl.Book) 
                        .ThenInclude(b => b.Publisher) 
                    .Include(wl => wl.Book) 
                        .ThenInclude(b => b.BookContentPhotos)
                    .OrderByDescending(wl => wl.CreatedAt)
                    .ToListAsync();

                var wishlistDtos = wishlistEntities.Select(wl => wl.ToWishlistItemDto()).ToList();

                return Ok(wishlistDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wishlist for user {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving wishlist.");
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status409Conflict)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WishlistItemDto>> AddBookToWishlist([FromBody] AddBookToWishlistDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or user ID not found.");
            }

            try
            {
                if (!await _wishlistService.CheckIfBookExists(dto.BookId))
                {
                    return NotFound($"Book with ID '{dto.BookId}' not found.");
                }

                if (await _wishlistService.CheckIfAlreadyWishlisted(userId, dto.BookId))
                {
                    return Conflict($"Book with ID '{dto.BookId}' is already in your wishlist.");
                }

                var wishlistItemEntity = await _wishlistService.AddBookToWishlistAsync(userId, dto.BookId);
                await _wishlistService.saveChangesAsync();
                if (wishlistItemEntity == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add book to wishlist due to an unexpected issue.");
                }

                var loadedWishlistItem = await (await _wishlistService.GetQueryableAsync())
                    .Where(wl => wl.Id == wishlistItemEntity.Id)
                    .Include(wl => wl.Book)
                        .ThenInclude(b => b.BookAuthors)
                            .ThenInclude(ba => ba.Author)
                    .Include(wl => wl.Book)
                        .ThenInclude(b => b.Publisher)
                    .Include(wl => wl.Book)
                        .ThenInclude(b => b.BookContentPhotos)
                    .FirstOrDefaultAsync(); 

                var wishlistItemDto = loadedWishlistItem?.ToWishlistItemDto();

                return CreatedAtAction(nameof(GetUserWishlist), new { }, wishlistItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book {BookId} to wishlist for user {UserId}.", dto.BookId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding book to wishlist.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveWishlistItem([FromRoute] int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or user ID not found.");
            }

            try
            {
                var wishlistItemToRemove = await (await _wishlistService.GetQueryableAsync())
                    .FirstOrDefaultAsync(wl => wl.Id == id && wl.ApplicationUserId == userId);

                if (wishlistItemToRemove == null)
                {
                    return NotFound($"Wishlist item with ID '{id}' not found or does not belong to this user.");
                }

                var result = await _wishlistService.RemoveWishlistItemAsync(userId, wishlistItemToRemove);
                await _wishlistService.saveChangesAsync();

                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing wishlist item {WishlistItemId} for user {UserId}.", id, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing wishlist item.");
            }
        }
    }
}
