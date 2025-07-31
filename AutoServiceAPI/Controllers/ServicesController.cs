using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoServiceAPI.Data;
using AutoServiceAPI.DTOs;
using AutoServiceAPI.Models;

namespace AutoServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/services")]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly AutoServiceDbContext _context;

        public ServicesController(AutoServiceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceRequest request)
        {
            // Check if service with the same name already exists
            var existingService = await _context.Services.FirstOrDefaultAsync(s => s.Name == request.Name);
            if (existingService != null)
            {
                return BadRequest(new { message = "Service with this name already exists" });
            }

            var service = new Service
            {
                Name = request.Name
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            var response = new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                CreatedAt = service.CreatedAt
            };

            return CreatedAtAction(nameof(CreateService), response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            var services = await _context.Services
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(services);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceDto>> UpdateService(string id, [FromBody] UpdateServiceRequest request)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound(new { message = "Service not found" });
            }

            // Check if service with the same name already exists (excluding current service)
            var existingService = await _context.Services.FirstOrDefaultAsync(s => s.Name == request.Name && s.Id != id);
            if (existingService != null)
            {
                return BadRequest(new { message = "Service with this name already exists" });
            }

            service.Name = request.Name;
            await _context.SaveChangesAsync();

            var response = new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                CreatedAt = service.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteService(string id)
        {
            var service = await _context.Services
                .Include(s => s.BillServices)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound(new { message = "Service not found" });
            }

            // Check if service is used in any bills
            if (service.BillServices.Any())
            {
                return BadRequest(new { message = "Cannot delete service that is used in bills" });
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}