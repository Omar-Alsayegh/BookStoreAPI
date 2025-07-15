using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs
{
    public class RentalRequestDto
    {
        [Required(ErrorMessage = "Book ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Book ID must be a positive integer.")]
        public int BookId { get; set; }

        [Range(1, 30, ErrorMessage = "Rental duration must be between 1 and 30 days.")]
        public int? DesiredDurationDays { get; set; } = 7;
    }
}
