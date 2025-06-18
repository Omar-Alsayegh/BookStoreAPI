using BookStoreApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookStoreApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Rentals = new HashSet<Rental>();
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateJoined { get; set; }

        public DateTime? LastPasswordResetRequestUtc { get; set; }
        public ICollection<Rental> Rentals { get; set; }
    }
}