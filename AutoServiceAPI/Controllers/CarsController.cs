using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoServiceAPI.Data;
using AutoServiceAPI.DTOs;
using AutoServiceAPI.Models;

namespace AutoServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/cars")]
    [Authorize]
    public class CarsController : ControllerBase
    {
        private readonly AutoServiceDbContext _context;

        public CarsController(AutoServiceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CarDto>> CreateCar([FromBody] CreateCarRequest request)
        {
            // Check if client exists
            var client = await _context.Clients.FindAsync(request.ClientId);
            if (client == null)
            {
                return BadRequest(new { message = "Client not found" });
            }

            // Check if VIN already exists
            var existingCar = await _context.Cars.FirstOrDefaultAsync(c => c.Vin == request.Vin);
            if (existingCar != null)
            {
                return BadRequest(new { message = "Car with this VIN already exists" });
            }

            var car = new Car
            {
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                Color = request.Color,
                Vin = request.Vin,
                LicensePlate = request.LicensePlate,
                InsuranceNumber = request.InsuranceNumber,
                Mileage = request.Mileage,
                EngineType = request.EngineType,
                Transmission = request.Transmission,
                NumberOfDoors = request.NumberOfDoors,
                ClientId = request.ClientId
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            var response = new CarDto
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Color = car.Color,
                Vin = car.Vin,
                LicensePlate = car.LicensePlate,
                InsuranceNumber = car.InsuranceNumber,
                Mileage = car.Mileage,
                EngineType = car.EngineType,
                Transmission = car.Transmission,
                NumberOfDoors = car.NumberOfDoors,
                ClientId = car.ClientId,
                CreatedAt = car.CreatedAt
            };

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetCars(
            [FromQuery] string? vin = null,
            [FromQuery] string? licensePlate = null)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(vin))
            {
                query = query.Where(c => c.Vin.Contains(vin));
            }

            if (!string.IsNullOrEmpty(licensePlate))
            {
                query = query.Where(c => c.LicensePlate != null && c.LicensePlate.Contains(licensePlate));
            }

            var cars = await query
                .Select(c => new CarDto
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Year = c.Year,
                    Color = c.Color,
                    Vin = c.Vin,
                    LicensePlate = c.LicensePlate,
                    InsuranceNumber = c.InsuranceNumber,
                    Mileage = c.Mileage,
                    EngineType = c.EngineType,
                    Transmission = c.Transmission,
                    NumberOfDoors = c.NumberOfDoors,
                    ClientId = c.ClientId,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCar(string id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }

            var response = new CarDto
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Color = car.Color,
                Vin = car.Vin,
                LicensePlate = car.LicensePlate,
                InsuranceNumber = car.InsuranceNumber,
                Mileage = car.Mileage,
                EngineType = car.EngineType,
                Transmission = car.Transmission,
                NumberOfDoors = car.NumberOfDoors,
                ClientId = car.ClientId,
                CreatedAt = car.CreatedAt
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CarDto>> UpdateCar(string id, [FromBody] UpdateCarRequest request)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }

            // Check if client exists
            var client = await _context.Clients.FindAsync(request.ClientId);
            if (client == null)
            {
                return BadRequest(new { message = "Client not found" });
            }

            // Check if VIN already exists (excluding current car)
            var existingCar = await _context.Cars.FirstOrDefaultAsync(c => c.Vin == request.Vin && c.Id != id);
            if (existingCar != null)
            {
                return BadRequest(new { message = "Car with this VIN already exists" });
            }

            car.Make = request.Make;
            car.Model = request.Model;
            car.Year = request.Year;
            car.Color = request.Color;
            car.Vin = request.Vin;
            car.LicensePlate = request.LicensePlate;
            car.InsuranceNumber = request.InsuranceNumber;
            car.Mileage = request.Mileage;
            car.EngineType = request.EngineType;
            car.Transmission = request.Transmission;
            car.NumberOfDoors = request.NumberOfDoors;
            car.ClientId = request.ClientId;

            await _context.SaveChangesAsync();

            var response = new CarDto
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Color = car.Color,
                Vin = car.Vin,
                LicensePlate = car.LicensePlate,
                InsuranceNumber = car.InsuranceNumber,
                Mileage = car.Mileage,
                EngineType = car.EngineType,
                Transmission = car.Transmission,
                NumberOfDoors = car.NumberOfDoors,
                ClientId = car.ClientId,
                CreatedAt = car.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCar(string id)
        {
            var car = await _context.Cars
                .Include(c => c.Bills)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }

            // Check if car has associated bills
            if (car.Bills.Any())
            {
                return BadRequest(new { message = "Cannot delete car with associated bills" });
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}