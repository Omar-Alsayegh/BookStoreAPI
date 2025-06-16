using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Response
{
    public class UpdateAuthorDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime Birthdate { get; set; }
    }
}
