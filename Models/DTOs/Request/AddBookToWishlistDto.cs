using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Request
{
    public class AddBookToWishlistDto
    {
        [Required(ErrorMessage = "Book ID is required.")]
        public int BookId { get; set; }

    }
}
