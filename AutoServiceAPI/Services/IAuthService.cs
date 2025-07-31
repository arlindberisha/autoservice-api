using AutoServiceAPI.Models;

namespace AutoServiceAPI.Services
{
    public interface IAuthService
    {
        Task<(User user, string token)> RegisterAsync(string organisationName, string email, string password, string ownerFirstName, string ownerLastName, string ownerPhone);
        Task<(User user, string token)> LoginAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> IsFirstUserAsync();
    }
} 