using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Models;

namespace BookStoreApi.Repositories
{
    public interface IRentalRepository:IGenericRepository<Rental>
    {
        Task<IEnumerable<Rental>> GetAllRentalsWithDetailsAsync(RentalQueryObject query);
        Task<IEnumerable<Rental>> GetRentalsForUserWithDetailsAsync(string userId);
        Task<Rental?> GetRentalByIdWithDetailsAsync(int rentalId); 
        Task<Rental?> GetPendingOrAcceptedRentalByUserId(string userId);
    }
}
