using BookStoreApi.Models;

namespace BookStoreApi.Services
{
    public interface ITokenService
    {
        string CreateJwtToken(ApplicationUser user, List<string> roles);
    }
}
