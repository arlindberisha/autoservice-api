using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoServiceAPI.Data;
using AutoServiceAPI.DTOs;
using AutoServiceAPI.Models;

namespace AutoServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/clients")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly AutoServiceDbContext _context;

        public ClientsController(AutoServiceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientRequest request)
        {
            var client = new Client
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var response = new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                CreatedAt = client.CreatedAt
            };

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients(
            [FromQuery] string? name = null,
            [FromQuery] string? phoneNumber = null)
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => (c.FirstName + " " + c.LastName).Contains(name) ||
                                       c.FirstName.Contains(name) ||
                                       c.LastName.Contains(name));
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                query = query.Where(c => c.PhoneNumber.Contains(phoneNumber));
            }

            var clients = await query
                .Select(c => new ClientDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(string id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            var response = new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                CreatedAt = client.CreatedAt
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(string id, [FromBody] UpdateClientRequest request)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            client.FirstName = request.FirstName;
            client.LastName = request.LastName;
            client.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            var response = new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                CreatedAt = client.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(string id)
        {
            var client = await _context.Clients
                .Include(c => c.Cars)
                .Include(c => c.Bills)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            // Check if client has associated cars or bills
            if (client.Cars.Any() || client.Bills.Any())
            {
                return BadRequest(new { message = "Cannot delete client with associated cars or bills" });
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}