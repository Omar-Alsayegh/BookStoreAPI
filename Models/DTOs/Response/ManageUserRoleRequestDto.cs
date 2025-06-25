using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Response
{
    public class ManageUserRoleRequestDto
    {
        [Required(ErrorMessage = "User identifier (email or username) is required.")]
        public string UserIdentifier { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Role name is required.")]
        public string RoleName { get; set; } = string.Empty;

    }
}
