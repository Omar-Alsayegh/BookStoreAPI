using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs
{
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Range(1000, 2030)]
        public int PublicationYear { get; set; }
        [Required]
        [Range(0, 10000)]
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
