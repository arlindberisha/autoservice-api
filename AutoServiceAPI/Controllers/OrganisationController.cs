using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoServiceAPI.Data;
using AutoServiceAPI.DTOs;
using AutoServiceAPI.Models;
using BCrypt.Net;

namespace AutoServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [Authorize]
    public class OrganisationController : ControllerBase
    {
        private readonly AutoServiceDbContext _context;

        public OrganisationController(AutoServiceDbContext context)
        {
            _context = context;
        }

        [HttpGet("organisation")]
        public async Task<ActionResult<OrganisationDto>> GetOrganisation()
        {
            var organisationId = User.FindFirstValue("OrganisationId");
            if (string.IsNullOrEmpty(organisationId))
            {
                return BadRequest(new { message = "Organisation ID not found in token" });
            }

            var organisation = await _context.Organisations.FindAsync(organisationId);
            if (organisation == null)
            {
                return NotFound(new { message = "Organisation not found" });
            }

            var response = new OrganisationDto
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Location = organisation.Location,
                SubscriptionStartDate = organisation.SubscriptionStartDate,
                SubscriptionDueDate = organisation.SubscriptionDueDate,
                CreatedAt = organisation.CreatedAt
            };

            return Ok(response);
        }

        [HttpPut("organisation")]
        [Authorize(Roles = "Organisation Owner")]
        public async Task<ActionResult<OrganisationDto>> UpdateOrganisation([FromBody] UpdateOrganisationRequest request)
        {
            var organisationId = User.FindFirstValue("OrganisationId");
            if (string.IsNullOrEmpty(organisationId))
            {
                return BadRequest(new { message = "Organisation ID not found in token" });
            }

            var organisation = await _context.Organisations.FindAsync(organisationId);
            if (organisation == null)
            {
                return NotFound(new { message = "Organisation not found" });
            }

            organisation.Name = request.Name;
            organisation.Location = request.Location;

            await _context.SaveChangesAsync();

            var response = new OrganisationDto
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Location = organisation.Location,
                SubscriptionStartDate = organisation.SubscriptionStartDate,
                SubscriptionDueDate = organisation.SubscriptionDueDate,
                CreatedAt = organisation.CreatedAt
            };

            return Ok(response);
        }

        [HttpPost("employees")]
        [Authorize(Roles = "Organisation Owner")]
        public async Task<ActionResult<UserDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            var organisationId = User.FindFirstValue("OrganisationId");
            if (string.IsNullOrEmpty(organisationId))
            {
                return BadRequest(new { message = "Organisation ID not found in token" });
            }

            // Check if user with email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            var employee = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Employee",
                OrganisationId = organisationId
            };

            _context.Users.Add(employee);
            await _context.SaveChangesAsync();

            var response = new UserDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = employee.Role,
                OrganisationId = employee.OrganisationId,
                CreatedAt = employee.CreatedAt
            };

            return CreatedAtAction(nameof(CreateEmployee), response);
        }

        [HttpGet("employees")]
        [Authorize(Roles = "Organisation Owner")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetEmployees()
        {
            var organisationId = User.FindFirstValue("OrganisationId");
            if (string.IsNullOrEmpty(organisationId))
            {
                return BadRequest(new { message = "Organisation ID not found in token" });
            }

            var employees = await _context.Users
                .Where(u => u.OrganisationId == organisationId && u.Role == "Employee")
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    OrganisationId = u.OrganisationId,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpDelete("employees/{id}")]
        [Authorize(Roles = "Organisation Owner")]
        public async Task<ActionResult> DeleteEmployee(string id)
        {
            var organisationId = User.FindFirstValue("OrganisationId");
            if (string.IsNullOrEmpty(organisationId))
            {
                return BadRequest(new { message = "Organisation ID not found in token" });
            }

            var employee = await _context.Users.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            if (employee.OrganisationId != organisationId)
            {
                return Forbid("You can only delete employees from your own organisation");
            }

            if (employee.Role == "Organisation Owner")
            {
                return BadRequest(new { message = "Cannot delete organisation owner" });
            }

            _context.Users.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}