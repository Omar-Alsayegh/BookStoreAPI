using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Response
{
    public class CreateAuthorDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
    }
}
