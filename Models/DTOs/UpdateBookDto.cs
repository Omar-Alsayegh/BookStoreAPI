using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs
{
    public class UpdateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int AuthorId { get; set; }

        [Range(1000, 2100)]
        public int PublicationYear { get; set; }

        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        [StringLength(10000000, MinimumLength = 0)]
        public string Content { get; set; }

        [Required]
        public int PublisherId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A book must have at least one author.")]
        public List<int> AuthorIds { get; set; } = new List<int>();
    }
}
