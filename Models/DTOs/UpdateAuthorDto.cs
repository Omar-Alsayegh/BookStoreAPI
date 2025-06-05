using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs
{
    public class UpdateAuthorDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(10)]
        public DateTime Birthdate { get; set; }
    }
}
