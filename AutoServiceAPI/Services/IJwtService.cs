using AutoServiceAPI.Models;

namespace AutoServiceAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
} 