using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }=string.Empty;
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
