using BookStoreApi.Entities;

namespace BookStoreApi.Services
{
    public interface IWishlistService
    {
        Task<Wishlist> AddBookToWishlistAsync(string userId, int bookId);
        Task<bool> RemoveWishlistItemAsync(string userId, Wishlist wishlistItem);
        Task<bool> CheckIfBookExists(int bookId); 
        Task<bool> CheckIfAlreadyWishlisted(string userId, int bookId);
        Task<IQueryable<Wishlist>> GetQueryableAsync();
        Task saveChangesAsync();
    }
}
