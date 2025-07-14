using BookStoreApi.Models;

namespace BookStoreApi.Entities
{
    public class Wishlist: BaseEntity
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
