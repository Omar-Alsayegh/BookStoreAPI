using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Response
{
    public class UpdatePublisherDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
