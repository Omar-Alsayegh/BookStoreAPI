namespace BookStoreApi.Models.DTOs.Auth
{
    public class LoginResponseDto
    {
        public LoginResponseDto()
        {
            Roles = new List<string>();
        }

        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JwtToken { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
