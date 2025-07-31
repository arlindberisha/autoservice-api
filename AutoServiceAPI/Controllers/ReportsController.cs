using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoServiceAPI.Data;
using AutoServiceAPI.DTOs;

namespace AutoServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/reports")]
    [Authorize(Roles = "Organisation Owner")]
    public class ReportsController : ControllerBase
    {
        private readonly AutoServiceDbContext _context;

        public ReportsController(AutoServiceDbContext context)
        {
            _context = context;
        }

        [HttpGet("income")]
        public async Task<ActionResult<IncomeReportDto>> GetIncomeReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate >= endDate)
            {
                return BadRequest(new { message = "Start date must be before end date" });
            }

            // Ensure we're working with UTC dates for consistency
            var utcStartDate = startDate.ToUniversalTime();
            var utcEndDate = endDate.ToUniversalTime();

            var bills = await _context.Bills
                .Where(b => b.Date >= utcStartDate && b.Date <= utcEndDate)
                .ToListAsync();

            var totalIncome = bills.Sum(b => b.TotalAmount);
            var totalBills = bills.Count;
            var totalDiscountGiven = bills.Where(b => b.DiscountValue.HasValue).Sum(b => 
            {
                if (b.DiscountType == "percentage")
                {
                    return b.Subtotal * (b.DiscountValue!.Value / 100);
                }
                else if (b.DiscountType == "fixed")
                {
                    return b.DiscountValue!.Value;
                }
                return 0;
            });

            var report = new IncomeReportDto
            {
                StartDate = utcStartDate,
                EndDate = utcEndDate,
                TotalIncome = totalIncome,
                TotalBills = totalBills,
                TotalDiscountGiven = totalDiscountGiven
            };

            return Ok(report);
        }
    }
}