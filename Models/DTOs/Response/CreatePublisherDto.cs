using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Response
{
    public class CreatePublisherDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
