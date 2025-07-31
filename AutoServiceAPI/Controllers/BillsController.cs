using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoServiceAPI.Data;
using AutoServiceAPI.DTOs;
using AutoServiceAPI.Models;
using AutoServiceAPI.Services;

namespace AutoServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/bills")]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly AutoServiceDbContext _context;
        private readonly IBillNumberService _billNumberService;

        public BillsController(AutoServiceDbContext context, IBillNumberService billNumberService)
        {
            _context = context;
            _billNumberService = billNumberService;
        }

        [HttpPost]
        public async Task<ActionResult<BillDto>> CreateBill([FromBody] CreateBillRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID not found in token" });
            }

            // Validate client exists
            var client = await _context.Clients.FindAsync(request.ClientId);
            if (client == null)
            {
                return BadRequest(new { message = "Client not found" });
            }

            // Validate car exists and belongs to client
            var car = await _context.Cars.FindAsync(request.CarId);
            if (car == null)
            {
                return BadRequest(new { message = "Car not found" });
            }
            if (car.ClientId != request.ClientId)
            {
                return BadRequest(new { message = "Car does not belong to the specified client" });
            }

            // Validate all services exist
            var serviceIds = request.ServicesPerformed.Select(s => s.ServiceId).ToList();
            var existingServices = await _context.Services
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();

            if (existingServices.Count != serviceIds.Count)
            {
                return BadRequest(new { message = "One or more services not found" });
            }

            // Calculate totals
            var subtotal = request.ServicesPerformed.Sum(s => s.Price);
            var discountAmount = 0m;
            
            if (request.Discount != null)
            {
                if (request.Discount.Type == "percentage")
                {
                    discountAmount = subtotal * (request.Discount.Value / 100);
                }
                else if (request.Discount.Type == "fixed")
                {
                    discountAmount = request.Discount.Value;
                }
            }

            var totalAmount = subtotal - discountAmount;
            if (totalAmount < 0) totalAmount = 0;

            // Generate bill number
            var billNumber = await _billNumberService.GenerateNextBillNumberAsync();

            // Create bill
            var bill = new Bill
            {
                BillNumber = billNumber,
                ClientId = request.ClientId,
                CarId = request.CarId,
                Date = request.Date,
                Subtotal = subtotal,
                DiscountValue = request.Discount?.Value,
                DiscountType = request.Discount?.Type,
                TotalAmount = totalAmount,
                Notes = request.Notes,
                CreatedById = userId
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            // Create bill services
            var billServices = request.ServicesPerformed.Select(s => new BillService
            {
                BillId = bill.Id,
                ServiceId = s.ServiceId,
                ServiceName = s.Name,
                Price = s.Price
            }).ToList();

            _context.BillServices.AddRange(billServices);
            await _context.SaveChangesAsync();

            var response = await GetBillDtoAsync(bill.Id);
            return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBills(
            [FromQuery] string? clientName = null,
            [FromQuery] string? licensePlate = null,
            [FromQuery] string? carType = null,
            [FromQuery] string? insuranceNumber = null,
            [FromQuery] DateTime? dateStart = null,
            [FromQuery] DateTime? dateEnd = null)
        {
            var query = _context.Bills
                .Include(b => b.Client)
                .Include(b => b.Car)
                .Include(b => b.BillServices)
                .AsQueryable();

            if (!string.IsNullOrEmpty(clientName))
            {
                query = query.Where(b => (b.Client.FirstName + " " + b.Client.LastName).Contains(clientName) ||
                                       b.Client.FirstName.Contains(clientName) ||
                                       b.Client.LastName.Contains(clientName));
            }

            if (!string.IsNullOrEmpty(licensePlate))
            {
                query = query.Where(b => b.Car.LicensePlate != null && b.Car.LicensePlate.Contains(licensePlate));
            }

            if (!string.IsNullOrEmpty(carType))
            {
                query = query.Where(b => (b.Car.Make + " " + b.Car.Model).Contains(carType) ||
                                       b.Car.Make.Contains(carType) ||
                                       b.Car.Model.Contains(carType));
            }

            if (!string.IsNullOrEmpty(insuranceNumber))
            {
                query = query.Where(b => b.Car.InsuranceNumber != null && b.Car.InsuranceNumber.Contains(insuranceNumber));
            }

            if (dateStart.HasValue)
            {
                query = query.Where(b => b.Date >= dateStart.Value);
            }

            if (dateEnd.HasValue)
            {
                query = query.Where(b => b.Date <= dateEnd.Value);
            }

            var bills = await query
                .Select(b => new BillDto
                {
                    Id = b.Id,
                    BillNumber = b.BillNumber,
                    ClientId = b.ClientId,
                    CarId = b.CarId,
                    Date = b.Date,
                    ServicesPerformed = b.BillServices.Select(bs => new ServicePerformedDto
                    {
                        ServiceId = bs.ServiceId,
                        Name = bs.ServiceName,
                        Price = bs.Price
                    }).ToList(),
                    Subtotal = b.Subtotal,
                    Discount = b.DiscountValue.HasValue ? new DiscountDto
                    {
                        Type = b.DiscountType ?? "",
                        Value = b.DiscountValue.Value
                    } : null,
                    TotalAmount = b.TotalAmount,
                    Notes = b.Notes,
                    CreatedById = b.CreatedById,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return Ok(bills);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BillDto>> GetBill(string id)
        {
            var billDto = await GetBillDtoAsync(id);
            if (billDto == null)
            {
                return NotFound(new { message = "Bill not found" });
            }

            return Ok(billDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BillDto>> UpdateBill(string id, [FromBody] UpdateBillRequest request)
        {
            var bill = await _context.Bills
                .Include(b => b.BillServices)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound(new { message = "Bill not found" });
            }

            // Validate client exists
            var client = await _context.Clients.FindAsync(request.ClientId);
            if (client == null)
            {
                return BadRequest(new { message = "Client not found" });
            }

            // Validate car exists and belongs to client
            var car = await _context.Cars.FindAsync(request.CarId);
            if (car == null)
            {
                return BadRequest(new { message = "Car not found" });
            }
            if (car.ClientId != request.ClientId)
            {
                return BadRequest(new { message = "Car does not belong to the specified client" });
            }

            // Validate all services exist
            var serviceIds = request.ServicesPerformed.Select(s => s.ServiceId).ToList();
            var existingServices = await _context.Services
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();

            if (existingServices.Count != serviceIds.Count)
            {
                return BadRequest(new { message = "One or more services not found" });
            }

            // Calculate totals
            var subtotal = request.ServicesPerformed.Sum(s => s.Price);
            var discountAmount = 0m;
            
            if (request.Discount != null)
            {
                if (request.Discount.Type == "percentage")
                {
                    discountAmount = subtotal * (request.Discount.Value / 100);
                }
                else if (request.Discount.Type == "fixed")
                {
                    discountAmount = request.Discount.Value;
                }
            }

            var totalAmount = subtotal - discountAmount;
            if (totalAmount < 0) totalAmount = 0;

            // Update bill
            bill.ClientId = request.ClientId;
            bill.CarId = request.CarId;
            bill.Date = request.Date;
            bill.Subtotal = subtotal;
            bill.DiscountValue = request.Discount?.Value;
            bill.DiscountType = request.Discount?.Type;
            bill.TotalAmount = totalAmount;
            bill.Notes = request.Notes;

            // Remove existing bill services
            _context.BillServices.RemoveRange(bill.BillServices);

            // Add new bill services
            var billServices = request.ServicesPerformed.Select(s => new BillService
            {
                BillId = bill.Id,
                ServiceId = s.ServiceId,
                ServiceName = s.Name,
                Price = s.Price
            }).ToList();

            _context.BillServices.AddRange(billServices);
            await _context.SaveChangesAsync();

            var response = await GetBillDtoAsync(bill.Id);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBill(string id)
        {
            var bill = await _context.Bills
                .Include(b => b.BillServices)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound(new { message = "Bill not found" });
            }

            _context.BillServices.RemoveRange(bill.BillServices);
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<BillDto?> GetBillDtoAsync(string billId)
        {
            return await _context.Bills
                .Where(b => b.Id == billId)
                .Select(b => new BillDto
                {
                    Id = b.Id,
                    BillNumber = b.BillNumber,
                    ClientId = b.ClientId,
                    CarId = b.CarId,
                    Date = b.Date,
                    ServicesPerformed = b.BillServices.Select(bs => new ServicePerformedDto
                    {
                        ServiceId = bs.ServiceId,
                        Name = bs.ServiceName,
                        Price = bs.Price
                    }).ToList(),
                    Subtotal = b.Subtotal,
                    Discount = b.DiscountValue.HasValue ? new DiscountDto
                    {
                        Type = b.DiscountType ?? "",
                        Value = b.DiscountValue.Value
                    } : null,
                    TotalAmount = b.TotalAmount,
                    Notes = b.Notes,
                    CreatedById = b.CreatedById,
                    CreatedAt = b.CreatedAt
                })
                .FirstOrDefaultAsync();
        }
    }
}