using Microsoft.EntityFrameworkCore;
using AutoServiceAPI.Data;
using AutoServiceAPI.Models;
using BCrypt.Net;

namespace AutoServiceAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AutoServiceDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(AutoServiceDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<(User user, string token)> RegisterAsync(string organisationName, string email, string password, string ownerFirstName, string ownerLastName, string ownerPhone)
        {
            // Check if user already exists
            var existingUser = await GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            // Create organisation
            var organisation = new Organisation
            {
                Name = organisationName,
                Location = "", // Can be updated later
                SubscriptionStartDate = DateTime.UtcNow,
                SubscriptionDueDate = DateTime.UtcNow.AddYears(1)
            };

            _context.Organisations.Add(organisation);
            await _context.SaveChangesAsync();

            // Create owner user
            var user = new User
            {
                FirstName = ownerFirstName,
                LastName = ownerLastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Organisation Owner",
                OrganisationId = organisation.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return (user, token);
        }

        public async Task<(User user, string token)> LoginAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = _jwtService.GenerateToken(user);
            return (user, token);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Organisation)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsFirstUserAsync()
        {
            var userCount = await _context.Users.CountAsync();
            return userCount == 0;
        }
    }
}