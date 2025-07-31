using Microsoft.AspNetCore.Mvc;
using AutoServiceAPI.Services;
using AutoServiceAPI.DTOs;

namespace AutoServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var (user, token) = await _authService.RegisterAsync(
                    request.OrganisationName,
                    request.Email,
                    request.Password,
                    request.OwnerFirstName,
                    request.OwnerLastName,
                    request.OwnerPhone
                );

                var response = new AuthResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    OrganisationId = user.OrganisationId,
                    Token = token,
                    Organisation = new OrganisationDto
                    {
                        Id = user.Organisation.Id,
                        Name = user.Organisation.Name,
                        Location = user.Organisation.Location,
                        SubscriptionStartDate = user.Organisation.SubscriptionStartDate,
                        SubscriptionDueDate = user.Organisation.SubscriptionDueDate,
                        CreatedAt = user.Organisation.CreatedAt
                    }
                };

                return CreatedAtAction(nameof(Register), response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (user, token) = await _authService.LoginAsync(request.Email, request.Password);

                var response = new AuthResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    OrganisationId = user.OrganisationId,
                    Token = token,
                    Organisation = new OrganisationDto
                    {
                        Id = user.Organisation.Id,
                        Name = user.Organisation.Name,
                        Location = user.Organisation.Location,
                        SubscriptionStartDate = user.Organisation.SubscriptionStartDate,
                        SubscriptionDueDate = user.Organisation.SubscriptionDueDate,
                        CreatedAt = user.Organisation.CreatedAt
                    }
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", details = ex.Message });
            }
        }
    }
}