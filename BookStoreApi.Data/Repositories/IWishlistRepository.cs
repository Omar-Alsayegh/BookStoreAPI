using BookStoreApi.Entities;
//using BookStoreApi.Models.DTOs.Request;
//using BookStoreApi.Models.DTOs.Response;

namespace BookStoreApi.Repositories
{
    public interface IWishlistRepository
    {
        Task AddAsync(Wishlist wishlistItem);
        void Remove(Wishlist wishlistItem); 
        Task<Wishlist> GetByIdAsync(int id);
        Task<IQueryable<Wishlist>> GetQueryable(); 
        Task SaveChangesAsync();
    }
}
