using BookStoreApi.Data;
using BookStoreApi.Entities;
using BookStoreApi.Models;
using BookStoreApi.Models.DTOs.Request;
using BookStoreApi.Models.DTOs.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Wishlist> _dbSet;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Wishlist>(); 
        }

        public async Task AddAsync(Wishlist wishlistItem)
        {
            await _dbSet.AddAsync(wishlistItem);
        }

        public async Task<Wishlist> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task<IQueryable<Wishlist>> GetQueryable()
        {
            return Task.FromResult(_dbSet.AsQueryable());
        }

        public void Remove(Wishlist wishlistItem)
        {
            _dbSet.Remove(wishlistItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
