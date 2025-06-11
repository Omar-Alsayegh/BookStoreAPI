using Microsoft.AspNetCore.Identity;

namespace BookStoreApi.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
}