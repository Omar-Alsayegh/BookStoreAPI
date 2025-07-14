using BookStoreApi.Entities;
using BookStoreApi.Models;
using BookStoreApi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IBookRepository _bookRepository; 
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistService(IWishlistRepository wishlistRepository, IBookRepository bookRepository, UserManager<ApplicationUser> userManager)
        {
            _wishlistRepository = wishlistRepository;
            _bookRepository = bookRepository;
            _userManager = userManager;
        }

        public async Task<Wishlist> AddBookToWishlistAsync(string userId, int bookId)
        {
            var wishlistItem = new Wishlist
            {
                ApplicationUserId = userId,
                BookId = bookId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = userId,
            };

            await _wishlistRepository.AddAsync(wishlistItem);
            return wishlistItem;
        }

        public async Task<bool> CheckIfAlreadyWishlisted(string userId, int bookId)
        {
            return await _wishlistRepository.GetQueryable()
                                        .Result.AnyAsync(wl => wl.ApplicationUserId == userId && wl.BookId == bookId);
        }

        public async Task<bool> CheckIfBookExists(int bookId)
        {
            return await _bookRepository.GetByIdAsync(bookId) != null;
        }

        public async Task<bool> RemoveWishlistItemAsync(string userId, Wishlist wishlistItem)
        {
            _wishlistRepository.Remove(wishlistItem);
            return true;
        }

        public async Task<IQueryable<Wishlist>> GetQueryableAsync()
        {
            return await _wishlistRepository.GetQueryable();
        }

        public async Task saveChangesAsync()
        {
            await _wishlistRepository.SaveChangesAsync();
        }
    }
}
