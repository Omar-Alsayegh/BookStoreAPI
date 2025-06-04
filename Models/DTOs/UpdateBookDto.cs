using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs
{
    public class UpdateBookDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Author { get; set; } = string.Empty;

        [Range(1000, 2100)]
        public int PublicationYear { get; set; }

        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        [StringLength(10000000, MinimumLength =0)]
        public string Content { get; set; }
    }
}
